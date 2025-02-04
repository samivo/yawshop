import React, { useEffect, useState } from "react";
import AddCircleIcon from '@mui/icons-material/AddCircle';
import RemoveCircleIcon from '@mui/icons-material/RemoveCircle';
import { TextField, Modal, Box, Grid2, InputAdornment, IconButton, FormGroup, FormControlLabel, Switch, Button, FormControl, InputLabel, MenuItem, Select, SelectChangeEvent, Avatar } from "@mui/material";
import { Editor } from '@tinymce/tinymce-react';
import { ApiEndpoint, ApiV1, Method } from "../../../Utilities/ApiFetch";
import { CustomerFieldType, ProductModel, ProductSpecificClientFields, ProductType } from "../../../Models/ProductModel";


declare global {
    interface Window {
      tinymce: any; // Use `any` or a more specific type if you have TinyMCE types available
    }
  }

const initialFormData: ProductModel = {
    id: 0,
    code: "",
    name: "",
    isActive: true,
    isVisibleToPublic: true,
    maxQuantityPerPurchase: null,
    quantityTotal: null,
    quantityUsed: 0,
    quantityLeft: null,
    priceInMinorUnitsIncludingVat: 0,
    vatPercentage: 0,
    shortDescription: "",
    descriptionOrInnerHtml: "",
    internalComment: "",
    availableFrom: new Date(),
    availableTo: null,
    productType: 0,
    customerFields: [],
    productGroupId: null,
    giftcardTargetProductCode: "",
    giftcardPeriodInDays: 0,
    createdAt: new Date(),
    modifiedAt: new Date(),
    modifier: null,
    avatarImage: "",
};

interface BasicModalProps {
    open: boolean;
    handleClose: () => void;
    product: ProductModel | null ;
    products: ProductModel[] | null;
}

const ProductFormModal: React.FC<BasicModalProps> = ({ open, handleClose, product, products }) => {


    const [dataFieldValue, SetDataFieldValue] = useState<string>("");
    const [formData, setFormData] = useState<ProductModel>(initialFormData);

    useEffect(() => {
        if (product) {
            setFormData(product);
        }
        else{
            setFormData(initialFormData);
        }
    }, [product]);
    

    const handleChange = (field: string ) => (event: React.ChangeEvent<HTMLInputElement>) => {

        if (event.target.type == "number" && event.target.value == "") {
            console.log("here");
            setFormData({ ...formData, [field]: null });
            return;
        }

        if (event.target.id == "productFieldValue") {

            SetDataFieldValue(event.target.value);
            return;
            
        }

        if (event.target.id == "availableTo" || event.target.id == "availableFrom") {

            if (event.target.value == "") {
                setFormData({ ...formData, [field]: null });
            }
        }

        if (field === "priceInMinorUnitsIncludingVat") {

            setFormData({ ...formData, priceInMinorUnitsIncludingVat: (Number.parseFloat(event.target.value) * 100) });
        }
        else {
            setFormData({ ...formData, [field]: event.target.value });
        }


    };

    const handleToggle = (data:any) => {
        setFormData({...formData,["isActive"]:data.target.checked});
    }

    const handleSelect = (event: SelectChangeEvent<string>) => {

        const { name, value } = event.target;

        setFormData({ ...formData, [name]:value });
    }

    const handleSubmit = async () => {

        //If there is product, update it otherwise add it
        if (product) {

            try {
                await ApiV1(ApiEndpoint.Product, Method.PUT, false, formData);
            } catch (error) {
                console.log(error);
                //show error to user?
                return;
            }

        }
        else {
            try {
                await ApiV1(ApiEndpoint.Product, Method.POST, false, formData);
            } catch (error) {
                console.log(error);
                //show error to user?
                return;
            }
        }

        handleClose();
    };

    const handleDelete = async () => {

        try {
            await ApiV1(ApiEndpoint.Product, Method.DELETE, false, null, `/${formData.code}`);
        } catch (error) {
            return;
        }

        handleClose();

    }

    const addDatafield = () => {
        
        if(product){
            var newFields: ProductSpecificClientFields = {
                id: 0,
                productModelId: product.id,
                fieldName : dataFieldValue,
                fieldType : CustomerFieldType.Text,
                isRequired: true,
                href: "",
            };
        }
        

        setFormData((data: ProductModel) => ({
            ...data,
            customerFields: [...(data.customerFields || []), newFields],
          }));
        
    }

    const removeDatafield = (fieldName: string) => {

        setFormData((prevData) => ({
            ...prevData,
            customerFields: (prevData.customerFields || []).filter(
                (field) => field.fieldName !== fieldName
            ),
        }));


    }

    const DatafieldComponent: React.FC<{ field: string }> = ({ field }) => {
        return (

            <Grid2 size={{ xs: 12, sm: 12, md: 3, lg: 2 }} sx={{display:'flex'}}>

                <TextField
                    id={field}
                    label={field}
                    variant="outlined"
                    size="small"
                    value={field}
                    disabled={true}
                />

                <IconButton onClick={()=>{removeDatafield(field)}} color="error">
                    <RemoveCircleIcon/>
                </IconButton>
            </Grid2>

        );
    };

    const openFilePicker = () => {
        const input = document.createElement('input');
        input.setAttribute('type', 'file');
        input.setAttribute('accept', 'image/*'); // Accept only image files
      
        input.onchange = function () {
          const file = input.files?.[0]; // Null-safe access to the selected file
          if (!file) {
            console.error('No file selected');
            return;
          }
      
          const reader = new FileReader();
      
          reader.onload = function (e) {
            const base64 = e.target?.result as string; // Get Base64 string
            console.log('Base64 Image:', base64);

            setFormData((prevData) => ({
                ...prevData,
                avatarImage: base64,
            }))
            
          };
      
          reader.readAsDataURL(file); // Convert file to Base64
        };
      
        input.click(); // Open the file picker dialog
      };

    return (

        <Modal
            open={open}
            onClose={handleClose}
            aria-labelledby="modal-modal-title"
            aria-describedby="modal-modal-description"
        >
            

            <Box sx={{
                display: 'flex',
                flexDirection: 'column',
                maxHeight: '90vh', // Limit height to 90% of viewport height
                width: '90%',
                maxWidth: '1200px',
                alignItems: 'center',
                justifyContent: 'flex-start', // Align content at the top
                position: 'fixed', // Anchor relative to viewport, not a container
                top: '5%', // Provide a top margin for visibility
                left: '50%', // Center horizontally
                transform: 'translateX(-50%)', // Only center horizontally
                bgcolor: 'background.paper',
                border: '2px solid #000',
                boxShadow: 24,
                p: 4,
                overflowY: 'auto', // Enable vertical scrolling
            }}>


                <Grid2 container size={12} spacing={2} sx={{}}>

                    <Grid2 container size={12} spacing={1}>

                        <Grid2 size={{ xs: 12, sm: 12, md: 4, lg: 2 }}>
                            <TextField
                                id="name"
                                label="Tuotteen nimi"
                                variant="outlined"
                                size="small"
                                value={formData.name}
                                onChange={handleChange("name")} 
                                slotProps={{
                                    inputLabel: {
                                      shrink: true,
                                    },
                                  }}/>
                        </Grid2>

                        <Grid2 size={{ xs: 12, sm: 12, md: 4, lg: 2 }}>
                            <FormControl fullWidth>
                                <InputLabel id="productType">Tyyppi</InputLabel>
                                <Select
                                    name="productType"
                                    labelId="demo-simple-select-label"
                                    id="demo-simple-select"
                                    value={formData.productType.toString()}
                                    size="small"
                                    label="Tyyppi"
                                    onChange={handleSelect}
                                >
                                    <MenuItem value={0}>Virtuaalinen</MenuItem>
                                    <MenuItem value={1}>Lahjakortti</MenuItem>
                                    <MenuItem value={2}>Tapahtuma</MenuItem>
                                </Select>
                            </FormControl>
                        </Grid2>

                        <Grid2 size={{ xs: 12, sm: 12, md: 4, lg: 2 }}>
                            <TextField
                                id="price"
                                label="Hinta (Sis alv)"
                                type="number"
                                variant="outlined"
                                size="small"
                                value={formData.priceInMinorUnitsIncludingVat === null ? "" : (formData.priceInMinorUnitsIncludingVat / 100)}
                                onChange={handleChange("priceInMinorUnitsIncludingVat")}
                                slotProps={{
                                    input: {
                                        endAdornment: <InputAdornment position="start">€</InputAdornment>
                                    },
                                    inputLabel:{
                                        shrink:true,
                                    }
                                }} />
                        </Grid2>

                        <Grid2 size={{ xs: 12, sm: 12, md: 4, lg: 2 }}>
                            <TextField
                                id="vat"
                                label="Alv"
                                type="number"
                                variant="outlined"
                                size="small"
                                value={formData.vatPercentage}
                                onChange={handleChange("vatPercentage")}
                                slotProps={{
                                    input: {
                                        endAdornment: <InputAdornment position="start">%</InputAdornment>
                                    },
                                    inputLabel:{
                                        shrink:true,
                                    }
                                }} />
                        </Grid2>

                        <Grid2 size={{ xs: 12, sm: 12, md: 4, lg: 2 }}>
                            <TextField
                                id="quantity"
                                label="Määrä"
                                type="number"
                                variant="outlined"
                                size="small"
                                value={formData.quantityTotal ?? ""}
                                onChange={handleChange("quantityTotal")}
                                slotProps={{
                                    input: {
                                        endAdornment: <InputAdornment position="start">kpl</InputAdornment>
                                    },
                                    inputLabel:{
                                        shrink:true,
                                    }
                                }}
                            />
                        </Grid2>

                        <Grid2 size={{ xs: 12, sm: 12, md: 4, lg: 2 }}>
                            <TextField
                                id="maxQuantity"
                                label="Kertaostos"
                                type="number"
                                variant="outlined"
                                size="small"
                                value={formData.maxQuantityPerPurchase ?? ""}
                                onChange={handleChange("maxQuantityPerPurchase")}
                                slotProps={{
                                    input: {
                                        endAdornment: <InputAdornment position="start">kpl</InputAdornment>
                                    },
                                    inputLabel:{
                                        shrink:true,
                                    }
                                }}
                                
                            />
                        </Grid2>
                    </Grid2>


                    <Grid2 container size={12} spacing={2}>

                        <Grid2 size={{ xs: 12, sm: 12, md: 12, lg: 3 }}>
                            <TextField
                                id="availableFrom"
                                label="Alkaen"
                                type="datetime-local"
                                variant="outlined"
                                size="small"
                                value={formData.availableFrom ?? ""}
                                onChange={handleChange("availableFrom")}
                                slotProps={{
                                    inputLabel: {
                                      shrink: true,
                                    },
                                  }}
                            />
                        </Grid2>

                        <Grid2 size={{ xs: 12, sm: 12, md: 12, lg: 3 }}>
                            <TextField
                                id="availableTo"
                                label="Päättyen"
                                type="datetime-local"
                                variant="outlined"
                                size="small"
                                value={formData.availableTo ?? ""}
                                onChange={handleChange("availableTo")}
                                slotProps={{
                                    inputLabel: {
                                      shrink: true,
                                    },
                                  }}
                            />
                        </Grid2>

                    </Grid2>

                    <Grid2 container size={12} spacing={2}>

                        <Grid2 size={12}>

                            <Button variant="contained" onClick={openFilePicker} >Lisää kuvake</Button>

                        </Grid2>

                        <Grid2 size={12}>

                            <Avatar alt="productAvatar" sx={{ width: 200, height: 200, borderRadius: '50%' }} src={formData.avatarImage || ""} />

                        </Grid2>
                    </Grid2>


                    <Grid2 container size={12} spacing={2}>

                        <Grid2 size={{ xs: 12, sm: 12, md: 12, lg: 8 }}>

                            <Editor
                                init={{
                                    menubar: true,
                                    plugins: 'advlist autolink lists link image charmap ',
                                }}
                                apiKey="cbnlpc6pajf392214l0g75a6hl0wxj7tpc5eh6c12y7aabks"
                                value={formData.descriptionOrInnerHtml ? formData.descriptionOrInnerHtml : ""}
                                onEditorChange={(newContent) =>
                                    setFormData((prevData) => ({
                                        ...prevData,
                                        descriptionOrInnerHtml: newContent,
                                    }))
                                }

                            />
                        </Grid2>

                    </Grid2>


                    <Grid2 container size={12} spacing={2} sx={{marginTop:'30px', display:'flex'}}>

                        <Grid2 size={{ xs: 12, sm: 12, md: 12, lg: 12 }}>

                            <TextField
                                id="productFieldValue"
                                label="Tietokentän nimi"
                                variant="outlined"
                                size="small"
                                value={dataFieldValue}
                                onChange={handleChange("productField")}
                                slotProps={{
                                    inputLabel: {
                                        shrink: true,
                                    },
                                }} />

                            <IconButton onClick={addDatafield} color="primary">
                                <AddCircleIcon/>
                            </IconButton>

                        </Grid2>


                    </Grid2>

                    <Grid2 container size={12} spacing={2}>

                        {formData.customerFields?.map((value: ProductSpecificClientFields) => (
                            <DatafieldComponent key={Math.random()} field={value.fieldName} />
                        ))}

                    </Grid2>


                    <Grid2 container size={12} spacing={2}
                        sx={{
                            marginTop: '20px'
                        }}>
                        {formData.productType === ProductType.Giftcard && (
                            <>
                                <Grid2 size={{ xs: 12, sm: 12, md: 4, lg: 2 }}
                                    sx={{
                                    }}>
                                    <TextField
                                        id="giftcardPeriodInDays"
                                        label="Lahjakortin kesto vrk"
                                        type="number"
                                        variant="outlined"
                                        size="small"
                                        value={formData.giftcardPeriodInDays ?? 0}
                                        onChange={handleChange("giftcardPeriodInDays")}
                                    />
                                </Grid2>

                                <Grid2 size={{ xs: 12, sm: 12, md: 4, lg: 2 }}
                                    sx={{
                                    }}>
                                    <FormControl fullWidth>
                                        <InputLabel id="giftcardTargetProductCode">Kohdetuote</InputLabel>
                                        <Select
                                            name="giftcardTargetProductCode"
                                            id="giftcardTargetProductCode"
                                            value={formData.giftcardTargetProductCode || ""}
                                            size="small"
                                            label="Kohdetuote"
                                            onChange={handleSelect}
                                        >
                                            {products?.map((product) => (
                                                <MenuItem key={product.code} value={product.code}>
                                                    {product.name}
                                                </MenuItem>
                                            ))}
                                        </Select>
                                    </FormControl>
                                </Grid2>
                            </>
                        )}
                    </Grid2>


                    <Grid2 container size={12} spacing={2}>
                        <Grid2 size={{ xs: 12, sm: 12, md: 4, lg: 3 }}>
                            <FormGroup>
                                <FormControlLabel
                                    id="test"
                                    control={<Switch  />}
                                    checked={formData.isActive}
                                    label="Tuote käytössä"
                                    onChange={handleToggle}
                                />
                            </FormGroup>
                        </Grid2>
                    </Grid2>


                    {product && (
                        <Grid2 container size={12} spacing={2}>
                            <Grid2 size={12}>
                                <h4>
                                    <a
                                        href={`${window.location.origin}/product/${formData.code}`}
                                        target="_blank"
                                        rel="noopener noreferrer"
                                        style={{ textDecoration: 'none', color: 'inherit' }}
                                    >
                                        {`${window.location.origin}/product/${formData.code}`}
                                    </a>
                                </h4>
                            </Grid2>
                        </Grid2>
                    )}
                    


                    <Grid2 container size={12} spacing={2}>
                        <Grid2 container size={12} sx={{ marginTop: '30px' }}>

                            <Grid2 size={product ? 4 : 6} sx={{ display: 'flex', justifyContent: 'center' }}>
                                <Button
                                    variant="contained"
                                    color="success"
                                    onClick={handleSubmit}>
                                    {product ? "Päivitä" : "Lisää"}
                                </Button>
                            </Grid2>

                            {product ? (
                                <Grid2 size={4} sx={{ display: 'flex', justifyContent: 'center' }}>
                                    <Button
                                        variant="contained"
                                        color="error"
                                        onClick={handleDelete}>
                                        Poista
                                    </Button>
                                </Grid2>
                            ) : null}

                            <Grid2 size={product ? 4 : 6} sx={{ display: 'flex', justifyContent: 'center' }}>
                                <Button
                                    variant="contained"
                                    color="primary"
                                    onClick={handleClose}>
                                    Sulje
                                </Button>
                            </Grid2>
                        </Grid2>
                    </Grid2>

                </Grid2>

            </Box>
        </Modal>
    );
};

export default ProductFormModal;
