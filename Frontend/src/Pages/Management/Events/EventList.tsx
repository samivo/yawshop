import { Button, Dialog, DialogActions, DialogContent, FormControlLabel, FormGroup, Grid2,  MenuItem,  Select, Switch } from "@mui/material";
import { DataGrid, getGridDateOperators, GRID_DATE_COL_DEF, GridColDef, GridColTypeDef, GridEditDateCell, GridFilterInputValueProps,  GridRowSelectionModel, GridToolbar, useGridApiRef } from "@mui/x-data-grid";
import { useEffect,  useState } from "react";
import { ProductModel, ProductType } from "../../../Models/ProductModel";
import { EventModel } from "../../../Models/EventModel";
import { ApiEndpoint, ApiV1, Method } from "../../../Utilities/ApiFetch";
import { DatePicker, DateTimePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFnsV3";
import { fi as locale } from 'date-fns/locale';
import { CheckoutModel, PaymentStatus } from "../../../Models/CheckoutModel";

const dateAdapter = new AdapterDateFns({ locale });

function GridFilterDateInput(
    props: GridFilterInputValueProps & { showTime?: boolean },
  ) {
    const { item, showTime, applyValue, apiRef } = props;

    const Component = showTime ? DateTimePicker : DatePicker;
  
    const handleFilterChange = (newValue: unknown) => {
      applyValue({ ...item, value: newValue });
    };
  
    return (
      <Component
        value={item.value ? new Date(item.value) : null}
        autoFocus
        label={apiRef.current?.getLocaleText('filterPanelInputLabel')}
        slotProps={{
          textField: {
            variant: 'standard',
          },
          inputAdornment: {
            sx: {
              '& .MuiButtonBase-root': {
                marginRight: -1,
              },
            },
          },
        }}
        onChange={handleFilterChange}
      />
    );
  }

  const dateColumnType: GridColTypeDef<Date, string> = {
    ...GRID_DATE_COL_DEF,
    resizable: false,
    renderEditCell: (params) => {
      return <GridEditDateCell {...params} />;
    },
    filterOperators: getGridDateOperators(false).map((item) => ({
      ...item,
      InputComponent: GridFilterDateInput,
      InputComponentProps: { showTime: false },
    })),
    valueFormatter: (value) => {
      if (value) {
        return dateAdapter.format(value, 'weekdayShort') + " "+ dateAdapter.format(value, 'keyboardDateTime24h') ;
      }
      return '';
    }
  };

export const EventList: React.FC = () => {

    const apiref = useGridApiRef();

    const [updateData, SetUpdateData] = useState<number>(0);
    const [products, SetProducts] = useState<ProductModel[]>([]);
    const [events, SetEvents] = useState<EventModel[]>([]);
    const [filteredEvents,SetFilteredEvents] = useState<EventModel[]>([]);
    const [checkouts, SetCheckouts] = useState<CheckoutModel[]>([]);
    const [gridColumns, SetGridColumns] = useState<GridColDef<EventModel>[]>([]);
    const [selectedEvents, SetSelectedEvents] = useState<GridRowSelectionModel>([]);
    const [showPastEvents, SetShowPastEvents] = useState(false);
    const [openDialog, setOpenDialog] = useState<{ text: string, open: boolean, type: "remove" | "cancel" | "undefined" }>({ text: "", open: false, type: "remove" });
    const [rowsModified, SetRowsModified] = useState<EventModel[]>([]);
    const [applyModifications, SetApplyModifications] = useState<{ apply: boolean, trigger: boolean }>({ apply: false, trigger: false });
    const [addEvent, SetAddEvent] = useState(false);
    const [removeEventsTrigger, SetRemoveEventsTrigger] = useState<number>(0);
    const [cancelEventsTrigger, SetCancelEventsTrigger] = useState<number>(0);

    


    useEffect(()=>{

        const fetchData = async () =>{
            let eventsResponse:EventModel[] = await ApiV1(ApiEndpoint.Event, Method.GET, false);
            let productsResponse: ProductModel[] = await ApiV1(ApiEndpoint.Product,Method.GET,false);
            let checkoutsResponse: CheckoutModel[] = await ApiV1(ApiEndpoint.Checkout, Method.GET, false);

            //Filter products
            SetProducts(productsResponse.filter(product => product.productType === ProductType.Event));
            
            //Show past events?
            if(!showPastEvents){
                SetFilteredEvents(filterPastEvents(eventsResponse, 43200000));
            }
            else{
                SetFilteredEvents(eventsResponse)
            }

            SetEvents(eventsResponse);

            //Filter checkouts
            SetCheckouts(checkoutsResponse.filter(checkout => eventsResponse.some(event => event.clientCode == checkout.client.code)));

        }

        fetchData();

    }, [updateData]);

    useEffect(()=>{

        //Update grid columns if products, events or checkouts gets updated

        //Get distinct client info columns
        let clientInfo = new Set<{ fieldName: string, fieldValue: string }>();

        //Loop events and client info and add fieldname to set.
        filteredEvents?.forEach(event => {
            event.client?.additionalInfo.forEach(info => {
                clientInfo.add({ fieldName: info.fieldName, fieldValue: info.fieldValue });
            });
        });

        let tempColumns: GridColDef<EventModel>[] = [];

        clientInfo.forEach(info => {

            tempColumns.push({
                field: info.fieldName,
                headerName:info.fieldName,
                valueGetter: (_value, row: EventModel) => {
                    return row.client?.additionalInfo.find(i => i.fieldName === info.fieldName)?.fieldValue;
                }

            });
            
        });

        //Add clients additional info to grid columns
        SetGridColumns([...columns, ...tempColumns]);


    }, [products, events, checkouts, filteredEvents]);

    useEffect(()=>{

        if(events == undefined){
            return;
        }

        let tempEvents: EventModel[] = [...events];

        if (showPastEvents === false) {

            tempEvents = filterPastEvents(tempEvents, 43200000);
        }

        tempEvents?.sort((a: EventModel, b: EventModel) => {

            let dateA = new Date(a.eventStart);
            let dateB = new Date(b.eventStart);

            if (dateA.getTime() > dateB.getTime()) {
                return -1;
            }
            else if (dateA.getTime() === dateB.getTime()) {
                return 0;
            }
            else {
                return 1;
            }
        });

        SetFilteredEvents(tempEvents);

    }, [showPastEvents]);

    useEffect(()=>{
    },[rowsModified]);

    useEffect(() => { 

        const remove = async () =>{
            SetRemoveEventsTrigger(0);

            const removableEvents = events.filter(e => selectedEvents.includes(e.code));

            if (removableEvents.find(e => e.clientCode != null)) {
                alert("Sisältää tapahtumia jotka on varattu. Peruuta ensin varaukset.");
                return;
            }

            removableEvents?.forEach(async event => {

                try {
                    await ApiV1(ApiEndpoint.Event, Method.DELETE, false, false, `/${event.code}`);
                    
                } catch (error) {
                    alert("Error while deleting events.");
                }
            });

            SetUpdateData(prevValue => prevValue + 1);

        }

        if(removeEventsTrigger > 0){
            remove();
        }
        

    }, [removeEventsTrigger]);

    useEffect(()=>{

        const cancel = async ()=>{
            SetCancelEventsTrigger(0);
            const eventToCancel = events.filter(e => selectedEvents.includes(e.code));

            //Backend will cancel registrations if clientcode is null
            eventToCancel.forEach(async event => {

                //If checkout has ongoing payment process, it should not to be cancelled
                const checkout = checkouts.find(c => c.products.some(p => p.eventCode === event.code));

                if (checkout?.paymentStatus === PaymentStatus.Initialized) {
                    alert("Ei voida peruuttaa tapahtumaa, maksuprosessi kesken.");
                    return;
                }

                event.clientCode = null;

                try {
                    await ApiV1(ApiEndpoint.Event, Method.PUT, false, event);
                } catch (error) {
                    alert("Registration cancel failed.");
                }
                
            });

            SetUpdateData(prevValue => prevValue + 1);
        }

        if (cancelEventsTrigger > 0) {
            cancel();
        }
        

    },[cancelEventsTrigger]);

    useEffect(()=>{

        const apply = async ()=>{

            if (applyModifications.apply === true) {

                try {

                    rowsModified.forEach(async row => {
                        
                        await ApiV1(ApiEndpoint.Event, Method.PUT, false, row);
                    });

                    SetRowsModified([]);
                    SetUpdateData(prevValue => prevValue + 1);
                    
                } catch (error) {
                    alert("Ei voitu tallentaa muutoksia.");
                }
            }
            else{
                SetRowsModified([]);
                SetUpdateData(prevValue => prevValue + 1);
            }

        }

        if (applyModifications.trigger === true) {
            apply();
            SetApplyModifications(prevValue => ({ ...prevValue, trigger: false }));
        }

    }, [applyModifications]);

    const createEvent = async (productCode: string) => {

        let startDate = new Date();
        let endDate = new Date();

        startDate.setHours(10, 0, 0, 0);
        endDate.setHours(10, 0, 0, 0);

        const newEvent: EventModel = {
            code: Math.random().toString(),
            productCode: productCode,
            client: null,
            clientCode: null,
            eventStart: startDate,
            eventEnd: endDate,
            hoursBeforeEventUnavailable: 0,
            isAvailable:false,
            isVisible:false,
        }

        try {
            let createdEvent: EventModel = await ApiV1(ApiEndpoint.Event, Method.POST, false, newEvent);
            SetFilteredEvents(prevValue => [createdEvent,...prevValue]);
            SetSelectedEvents([createdEvent.code]);

        } catch (error) {
            alert("Error while creating new event.");
        }
    }

    /**
     * Filter past events by events start time compared to current time.
     * @param events list of events
     * @param timeInMilliseconds Extends the event start time threshold, ensuring the event is included in the list.
     * Example: 43200000 (12h) -> Lists events that has not started yet and events that started within last 12 hours.
     * Negative value makes opposite.
     * @returns 
     */
    const filterPastEvents = (events: EventModel[], timeInMilliseconds: number): EventModel[] => {


        const filteredResult = events.filter(event => ((new Date(event.eventStart)).getTime() + timeInMilliseconds) > new Date().getTime());
        return filteredResult;
    }

    const handleRemoveDialog = (text: string) => {
        setOpenDialog({ text: text, open: true,type:"remove"});
    };

    const handleCancelDialog = (text: string) => {
        setOpenDialog({ text: text, open: true, type:"cancel" });
    };


    const handleCloseDialog = async (selection: boolean) => {
        
        setOpenDialog({ ...openDialog, open: false });

        if (selection === true) {

            if(openDialog.type === "cancel"){
                //Cancel selected events
                SetCancelEventsTrigger(prevValue => prevValue + 1);
                
            }
            else if (openDialog.type === "remove") {
                //Remove selected events
                SetRemoveEventsTrigger(prevValue => prevValue + 1);
            }
        }

    };

    const deleteEvents = () => {
        handleRemoveDialog("Valitut tapahtumat poistetaan.");
    }

    const cancelEvents = () => {
        handleCancelDialog("Valitut tapahtumat peruutetaan.");
    }


    const columns: GridColDef<EventModel>[] = [

        {
            field: 'productName',
            headerName: 'Tapahtuma',
            width: 150,
            type: "string",

            valueGetter: (_value, row) => {
                let test = products?.find(product => product.code === row.productCode)?.name;
                return test;
            },

            editable: false,
        },
        {
            field: 'eventStart',
            headerName: 'Alkaa',
            ...dateColumnType,
            width: 150,
            type: "dateTime",
            valueGetter: (value) => {
                return (new Date(value));
            },
            editable: true,
        },
        {
            field: 'eventEnd',
            headerName: 'Päättyy',
            ...dateColumnType,
            type: "dateTime",
            width: 150,
            valueGetter: (value) => {
                return (new Date(value))
            },
            editable: true
        },
        {
            field: 'hoursBeforeEventUnavailable',
            headerName: 'Ilm. päättyy',
            description: "Kuinka monta tuntia ennen tapahtuma alkua ilmoittautuminen päättyy",
            type: "string",
            editable: true,
            width: 100,
            valueFormatter: (value) => {
                return (value + " h");
            }
        },
        {
            field:"isVisible",
            headerName:'Julkinen',
            type:'boolean',
            editable:true,
        },
        {
            field: 'used',
            headerName: 'Tila',
            type: 'string',
            width: 110,
            valueGetter: (_value, row) => {

                if (row.clientCode != null) {
                    return "Varattu";
                }
                else if (row.isAvailable === true && row.clientCode == null) {
                    return "Vapaa";
                }
            },
            renderCell: (params) => {
                const text = params.value;
                let color = "black";

                if (text === "Varattu") {
                    color = "red";
                }
                else if (text === "Vapaa") {
                    color = "green";
                }

                return (
                    <span style={{ color: color, fontWeight: 'bold' }}>
                        {text}
                    </span>
                );
            },
        },
        {
            field: 'paymentStatus',
            headerName: 'Maksun tila',
            type: 'string',
            width: 100,
            valueGetter: (_value, row) => {
                const checkout = checkouts.find(c => c.products.some(p => p.eventCode === row.code));
                switch (checkout?.paymentStatus)
                {
                    case PaymentStatus.Initialized:
                        return "Kesken";
                    case PaymentStatus.Cancelled:
                        return "Hylätty";
                    case PaymentStatus.Fail:
                        return "Hylätty";
                    case PaymentStatus.Ok:
                        return "Maksettu";
                    case PaymentStatus.Delayed:
                        return "Viivettä";
                    case PaymentStatus.Pending:
                        return "Viivettä";

                }
            }
        },
        {
            field:'discount',
            headerName: 'Alennus',
            type:'string',
            align:'center',
            valueGetter:(_value,row)=>{
                const checkout = checkouts.find(c => c.products.some(p => p.eventCode === row.code));
                const checkoutProduct = checkout?.products.find(p => p.eventCode === row.code);

                return checkoutProduct?.discountCode ?  checkoutProduct?.discountCode : "Ei";

            }
            
        },
        {
            field:'giftcard',
            headerName: 'Lahjakortti',
            type:'string',
            align:'center',
            valueGetter: (_value, row) => {
                const checkout = checkouts.find(c => c.products.some(p => p.eventCode === row.code));
                const checkoutProduct = checkout?.products.find(p => p.eventCode === row.code);

                return checkoutProduct?.giftcardCode ?checkoutProduct?.giftcardCode:"Ei" ;
            }            
        },
        {
            field: 'name',
            headerName: 'Nimi',
            type: 'string',
            width: 140,
            valueGetter: (_value, row) => {
                if (row.client) {
                    return row.client.firstName + " " + row.client.lastName
                }
            }

        },
        {
            field: 'email',
            headerName: 'Sähköposti',
            type: 'string',
            width: 250,
            valueGetter: (_value, row) => {
                if (row.client) {
                    return row.client.email
                }
            }

        }
    ];


    return (
        <>
            <Grid2 container sx={{ display: 'flex', justifyContent: 'center', marginTop: '50px' }}>

                <Grid2 sx={{width: '100%' }}>

                    <Grid2 container spacing={2} sx={{marginBottom:'20px'}}>
                        <FormGroup>
                            <FormControlLabel sx={{ width: '175px' }} control={<Switch checked={showPastEvents} onChange={() => { SetShowPastEvents(prevValue => !prevValue) }} />} label="Näytä menneet" />
                        </FormGroup>

                        {addEvent === false && (
                            <Button variant="contained" size="small" onClick={() => { SetAddEvent(true) }}>Luo tapahtuma</Button>
                        )}

                        {rowsModified.length > 0 && (
                            <>
                                <Button variant="contained" size="small" color="success" onClick={() => { SetApplyModifications({ apply: true, trigger: true }) }}>Tallenna muutokset</Button>
                                <Button variant="contained" size="small" color="error" onClick={() => {SetApplyModifications({ apply: false, trigger: true })  }}>Hylkää muutokset</Button>
                            </>
                        )}

                        {addEvent &&
                            (
                                <>
                                    <Select size="small" value={0} onChange={(event) => {
                                        createEvent(event.target.value as string);
                                        SetAddEvent(false);
                                    }}>
                                        <MenuItem value={0} >Valitse tuote</MenuItem>
                                        {products.map((product) => (

                                            <MenuItem value={product.code}>{product.name}</MenuItem>

                                        ))}
                                    </Select>
                                </>
                            )}

                        {selectedEvents.length > 0 && rowsModified.length <= 0 && (
                            <>
                                <Button variant="contained" color="error" size="small" onClick={() => { deleteEvents() }}>Poista</Button>
                                <Button variant="contained" color="error" size="small" onClick={() => { cancelEvents() }}>peruuta</Button>
                            </>
                        )}

                    </Grid2>


                    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={locale}>
                        <DataGrid
                            
                            getRowId={(row) => { return row.code }}
                            rows={filteredEvents}
                            columns={gridColumns}
                            initialState={{
                                pagination: {
                                    paginationModel: {
                                        pageSize: 50,
                                    },
                                },
                                sorting: {
                                    sortModel: [{ field: 'eventStart', sort: 'desc' }]
                                },
                            }}
                            pageSizeOptions={[5, 10, 20, 50, 100]}
                            slots={{
                                toolbar: GridToolbar
                            }}
                            slotProps={{
                                toolbar: {
                                    showQuickFilter: true,

                                },
                            }}

                            sortingOrder={['desc', 'asc']}
                            checkboxSelection
                            disableRowSelectionOnClick
                            rowSelectionModel={selectedEvents}
                            onRowSelectionModelChange={(newRowSelectionModel) => {
                                SetSelectedEvents(newRowSelectionModel);
                            }}
                            editMode="row"
                            processRowUpdate={(newRow: EventModel) => {

                                //Apply row modifications
                                SetRowsModified((prevValue) => {

                                    const rowExists = prevValue.some(row => row.code === newRow.code);
                                  
                                      if (rowExists) {
                                        //If row modified already exists in table, raplace it
                                        return prevValue.map(row => row.code === newRow.code ? newRow : row);
                                    } else {
                                        //Push new row to list
                                        return [...prevValue, newRow];
                                    }
                                 });

                                return newRow;
                            }}
                            apiRef={apiref}
                        />
                    </LocalizationProvider>

                </Grid2>

                <Dialog
                    open={openDialog.open}
                    onClose={handleCloseDialog}
                    aria-labelledby="alert-dialog-title"
                    aria-describedby="alert-dialog-description"
                >
                    <DialogContent>
                        <strong style={{ color: 'red' }}>DANGER ZONE</strong> {openDialog.text} <strong style={{ color: 'red' }}>DANGER ZONE</strong>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => { handleCloseDialog(false) }} color="primary" variant="contained" >Peruuta</Button>
                        <Button onClick={() => { handleCloseDialog(true) }} color="error" variant="contained">OK</Button>
                    </DialogActions>
                </Dialog>


            </Grid2>

        </>
    );

}