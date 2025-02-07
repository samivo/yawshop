import Box from '@mui/material/Box';
import { DataGrid, getGridDateOperators, GRID_DATE_COL_DEF,  gridClasses, GridColDef, GridColTypeDef, GridEditDateCell, GridFilterInputValueProps, GridRenderCellParams,  GridToolbar } from '@mui/x-data-grid';
import { useEffect, useState } from 'react';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFnsV3';
import { fi as locale } from 'date-fns/locale';
import { DateTimePicker, DatePicker } from '@mui/x-date-pickers';
import { Button, Grid2, IconButton } from '@mui/material';
import ProductFormModal from './ProductForm';
import EditIcon from '@mui/icons-material/Edit';
import { ApiEndpoint, ApiV1, Method } from '../../../Utilities/ApiFetch';
import { ProductModel, ProductType } from '../../../Models/ProductModel';


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
      return dateAdapter.format(value, 'keyboardDate');
    }
    return '';
  },
};


export default function ProductList() {

  const [updateData, SetUpdateData] = useState<number>();
  const [modalOpen, setModalOpen] = useState(false);
  const [product, SetProduct] = useState<ProductModel | null>(null);
  const [rows, setRows] = useState<any>();
  
    // Use the useEffect hook to run the function when the component mounts
    useEffect(() => {

      const fetch = async () => {

        var prods: ProductModel[] = await ApiV1(ApiEndpoint.Product, Method.GET, false);
    
        setRows(prods);
      };

      fetch();

    }, [updateData]);

    const handleOpen = (Product: ProductModel | null) => {

      SetProduct(Product);
      
      setModalOpen(true)
    };
  
    const handleClose = () => {
      setModalOpen(false); 
    }

  const columns: GridColDef<(typeof rows)[number]> [] = [
    {
      field: 'name',
      headerName: 'Tuote',
      width: 150,
    },
    {
      field:'code',
      headerName:'Koodi',
      type:'string',
      width:120
      
    },
    {
      field: 'productType',
      headerName: 'Tyyppi',
      valueGetter: (paramas : ProductType) =>{
        if(paramas == ProductType.Event){
          return "Tapahtuma";
        }
        else if(paramas == ProductType.Giftcard){
          return "Lahjakortti";
        }
        else if(paramas == ProductType.Virtual){
          return "Normaali";
        }
      },
      width: 150,
    },
    {
      field:'quantities',
      headerName:'Ostettu/Jäljellä',
      type:'string',
      width:120,
      valueGetter: (_value : any, row: ProductModel) => {

        if(row.quantityTotal == null){
          return row.quantityUsed +" / "+ "-";
        }
        return row.quantityUsed + " / "+row.quantityTotal;
      }
      
    },
    {
      field: 'isActive',
      headerName: 'Käytössä',
      type: 'boolean',
      width: 130,
    },
    {
      field: 'availableFrom',
      ...dateColumnType,
      headerName: 'Myynti alkaa',
      width: 130,
      valueGetter: (value) => {
        return (new Date(value));
      }
    },

    {
      field: 'availableTo',
      ...dateColumnType,
      headerName: 'Päättyy',
      width: 120,
      valueGetter: (value) => {

        if (value == null) {
          return null;
        }
        return (new Date(value));

      }
    },
    {
      field: 'modifier',
      headerName: 'Muokannut',
      type: 'string',
      width: 140,
    },
    {
      field: 'edit',
      headerName: '',
      width: 10,
      sortable:false,
      renderCell: (params: GridRenderCellParams<any>)=>{
        return(
          <IconButton onClick={()=>handleOpen(params.row)}>
            <EditIcon/>
          </IconButton>
        );
      }
    },
  ];

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%', marginTop: '20px' }}>

      <Grid2 container spacing={1}>

        <Grid2 size={12}>
          <Button variant="contained" color="success" onClick={()=>handleOpen(null)} >
            Luo uusi
          </Button>
        </Grid2>

        <ProductFormModal open={modalOpen} updateProductList={SetUpdateData} handleClose={handleClose} product={product} products={rows} />

        <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={locale}>
          <DataGrid
            rows={rows}
            columns={columns}
            slots={{
              toolbar: GridToolbar
            }}
            initialState={{
              pagination: {
                paginationModel: {
                  pageSize: 10,
                },
              },
            }}
            pageSizeOptions={[5, 10, 20, 50, 100]}
            disableRowSelectionOnClick
            sx={{
              maxWidth: '1300px', [`& .${gridClasses.cell}:focus, & .${gridClasses.cell}:focus-within`]: {
                outline: 'none',
              }
            }}

          />
        </LocalizationProvider>

      </Grid2>

    </Box>

  );
}