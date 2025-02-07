import { Button, Chip,  Dialog,  DialogActions,  DialogContent,  DialogContentText,  DialogTitle,  FormControlLabel,  FormGroup,  Grid2, IconButton, Switch } from "@mui/material";
import { DataGrid, getGridDateOperators, GRID_DATE_COL_DEF,  gridClasses, GridColDef, GridColTypeDef, GridEditDateCell, GridFilterInputValueProps, GridRenderCellParams, GridToolbar, GridValueGetter } from "@mui/x-data-grid";
import { useEffect, useRef, useState } from "react";
import { ProductModel } from "../../../Models/ProductModel";
import { EventModel } from "../../../Models/EventModel";
import { ApiEndpoint, ApiV1, Method } from "../../../Utilities/ApiFetch";
import { EventStatus } from "../../../Utilities/EventModelPublic";
import { DatePicker, DateTimePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFnsV3";
import { fi as locale } from 'date-fns/locale';
import DeleteIcon from '@mui/icons-material/Delete';

const dateAdapter = new AdapterDateFns({ locale });

//Copypasted from depths of internet to make the dates look a like human readable ddMMYYY
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
        return dateAdapter.format(value, 'keyboardDateTime24h');
      }
      return '';
    },
  };

export const RegistrationsList: React.FC = () => {

    const [products, SetProducts] = useState<ProductModel[]>();
    const [events, SetEvents] = useState<EventModel[]>();
    const [filteredEvents,SetFilteredEvents] = useState<EventModel[]>();
    const [showPastEvents, SetShowPastEvents] = useState(false);
    const [openDialog, setOpenDialog] = useState(false);
    const deleteEventCode = useRef("");

    useEffect(()=>{

        const fetchData = async () =>{
            let events:EventModel[] = await ApiV1(ApiEndpoint.Event, Method.GET, false);
            let products = await ApiV1(ApiEndpoint.Product,Method.GET,false);

            if(!showPastEvents){
                SetFilteredEvents(events.filter(event => event.status !== EventStatus.Expired));
            }

            SetEvents(events);
            SetProducts(products);
        }

        fetchData();

    }, []);

    //Filter past events
    useEffect(()=>{

        if(!showPastEvents){
            SetFilteredEvents(events?.filter(event => event.status !== EventStatus.Expired));
        }
        else{
            SetFilteredEvents(events);
        }

    }, [showPastEvents]);

    const handleClickOpenDialog = () => {
        setOpenDialog(true);
    };

    const handleCloseDialog = async (selection: boolean) => {
        setOpenDialog(false);
        if (selection === true) {
            await ApiV1(ApiEndpoint.Event, Method.DELETE, false, null, `/${deleteEventCode.current}`);
        }

    };


    const deleteEvent = (eventCode: string) => {
        deleteEventCode.current = eventCode;
        handleClickOpenDialog();
    }

    const columns: GridColDef<EventModel>[] = [
        
        {
            field: 'productName',
            headerName: 'Tapahtuma',
            width: 150,
            align:'center',
            type: "string",
            valueGetter: (_value, row: EventModel) => {
                return products?.find(product => product.code === row.productCode)?.name;
            }
            
        },
        {
            field: 'eventStart',
            headerName: 'Alkaa',
            ...dateColumnType,
            width: 150,
            type: "string",
            valueGetter: (value) => {
                return (new Date(value))
            }
        },
        {
            field: 'eventEnd',
            headerName: 'Päättyy',
            ...dateColumnType,
            width: 150,
            valueGetter: (value) => {
                return (new Date(value))
            }
        },
        {
            field: 'usage',
            headerName: 'Osallistujat',
            type: 'string',
            width: 110,
            valueGetter: (_value, row: EventModel) => {
                return row.registrationsQuantityUsed +"/"+(row.registrationsQuantityTotal ? row.registrationsQuantityTotal : "");
            }
        },
        {
            field: 'status',
            headerName: 'Tila',
            type:'custom',
            width: 110,
            renderCell: (params: GridRenderCellParams<EventModel>) => {

                let chipColor: "default" | "primary" | "secondary" | "error" | "info" | "" | "success" | "warning" = "default";
                let chipLabel: string = "";

                switch (params.row.status) {
                    case EventStatus.Available:
                        chipColor = "success";
                        chipLabel = "Vapaa";
                        break;
                    case EventStatus.Full:
                        chipColor = "primary";
                        chipLabel = "Täynnä";
                        break;
                    case EventStatus.Expired:
                        chipColor = "";
                        chipLabel = "Mennyt";
                        break;

                }
               
                return(
                    <>
                        <Chip label={chipLabel} color={(chipColor != "" ? chipColor : undefined)} variant="filled" />
                    </>
                );
            }
            
        },
        {
            field: 'delete',
            headerName: 'Poista',
            width: 110,
            type:'custom',
            renderCell: (params: GridRenderCellParams<EventModel>)=>{
                return(
                    <IconButton onClick={() => { deleteEvent(params.row.code) }}><DeleteIcon color="error"></DeleteIcon></IconButton>
                )
            }
        },
    ];


    return (
        <>
            <Grid2 container sx={{ display: 'flex', justifyContent: 'center', marginTop: '50px' }}>

                <Grid2 sx={{ width: '100%', maxWidth: '1200px' }}>

                <FormGroup>
                        <FormControlLabel control={<Switch checked={showPastEvents} onChange={() => { SetShowPastEvents(prevValue => !prevValue) }} />} label="Näytä menneet" />
                </FormGroup>

                    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={locale}>
                        <DataGrid
                            rows={filteredEvents}
                            columns={columns}
                            initialState={{
                                pagination: {
                                    paginationModel: {
                                        pageSize: 50,
                                    },
                                },
                                sorting: {
                                    sortModel: [{ field: 'eventStart', sort: 'desc' }]
                                }
                            }}
                            pageSizeOptions={[5, 10, 20, 50, 100]}
                            slots={{ toolbar: GridToolbar }}
                            sx={{
                                maxWidth: '1300px', [`& .${gridClasses.cell}:focus, & .${gridClasses.cell}:focus-within`]: {
                                    outline: 'none',
                                }
                            }}
                            sortingOrder={['desc', 'asc']}
                            disableRowSelectionOnClick
                            
                            
                        />
                    </LocalizationProvider>

                </Grid2>

                <Dialog
                    open={openDialog}
                    onClose={handleCloseDialog}
                    aria-labelledby="alert-dialog-title"
                    aria-describedby="alert-dialog-description"
                >
                    <DialogTitle id="alert-dialog-title">
                        {"Poistetaanko tapahtuma?"}
                    </DialogTitle>
                    <DialogContent>
                       <strong>HUOMIO</strong> Kaikki ilmoittautumiset tapahtumaan perutaan!
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={()=>{handleCloseDialog(false)}} color="primary" autoFocus >Peruuta</Button>
                        <Button onClick={()=>{handleCloseDialog(true)}} color="error">
                            Poista
                        </Button>
                    </DialogActions>
                </Dialog>

            </Grid2>

        </>
    );

}