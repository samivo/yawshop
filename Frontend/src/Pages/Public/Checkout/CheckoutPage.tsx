import { Alert, Avatar, Box, Button, Checkbox, CircularProgress, Divider, FormControl, FormControlLabel, FormGroup, FormHelperText, IconButton, Link, List, ListItem, ListItemAvatar, ListItemText, Snackbar, SnackbarCloseReason, Step, StepLabel, Stepper, SxProps, TextField, Typography } from "@mui/material";
import Grid from '@mui/material/Grid2';
import RemoveCircleIcon from '@mui/icons-material/RemoveCircle';
import AddCircleIcon from '@mui/icons-material/AddCircle';
import { ShoppingCartItem } from "../../../Utilities/ShoppingCartItem";
import React, { useEffect, useState } from "react";
import { DateToString } from "../../../Utilities/DateToString";
import { ApiEndpoint, ApiV1, Method } from "../../../Utilities/ApiFetch";
import { ClientModel, ShoppingCartModel } from "../../../Utilities/ShoppingCartModel";
import { Discount } from "../../../Utilities/DiscountModel";
import PaytrailImage from "../../../assets/2025-pankit-visa-mastercard-mobilepay.svg";
import { CheckoutResponse } from "../../../Utilities/CheckoutResponse";
import { ProductSpecificClientFields, ProductType } from "../../../Models/ProductModel";

const steps = [
    'Valitse tuote',
    'Kassa - Täytä tiedot',
    'Maksa',
];

const initialCustomerForm: ClientModel = {
    firstName: "",
    lastName: "",
    email: "",
    emailConfirmation: "",
    additionalInfo: []
}

interface FormErrorState {
    fieldName: string,
    errorText: string,
}

const initialErrorState: FormErrorState[] = [];

interface Props {

    cart: ShoppingCartItem[],
    SetShoppingCart: React.Dispatch<React.SetStateAction<ShoppingCartItem[]>>
    sx? : SxProps
}

const CartList: React.FC<Props> = (props) => {

    return (
        <List sx={props.sx}>

            {props.cart.map((cartItem, key) => {

                const setQuantity = (increase:boolean) => {

                    props.SetShoppingCart((prevValue) => {

                        let prevValueClone = structuredClone(prevValue);

                        prevValueClone.forEach(targetCartItem => {

                            //Multiproduct shopping cart -> since there can be different events with same product code, sum
                            //event based on event code
                            //There can't or should not be identical product codes in cart if product type is something else than event.
                            //Duplicate codes should be checked

                            if (cartItem.product.productType === ProductType.Event) {

                                if (cartItem.event?.code === targetCartItem.event?.code) {

                                    //Same event cant be purchased multiple times. Makes sense?
                                    increase ? targetCartItem.quantity++ : targetCartItem.quantity--;
                                    targetCartItem.quantity < 0 ? targetCartItem.quantity = 0 : null;
                                    targetCartItem.quantity > 1 ? targetCartItem.quantity = 1 : null;
                                }

                            }
                            else {
                                if (cartItem.product.code === targetCartItem.product.code) {

                                    increase ? targetCartItem.quantity++ : targetCartItem.quantity--;
                                    targetCartItem.quantity < 0 ? targetCartItem.quantity = 0 : null;

                                    if (targetCartItem.product.maxQuantityPerPurchase) {
                                        targetCartItem.quantity > targetCartItem.product.maxQuantityPerPurchase ?
                                            targetCartItem.quantity = targetCartItem.product.maxQuantityPerPurchase : null;
                                    }

                                }
                            }
                        });

                        return prevValueClone.filter(cartItem => cartItem.quantity > 0);
                        
                    });
                }

                let productName = cartItem.product.name;

                cartItem.event ? productName += `\n${DateToString.getDate(cartItem.event.eventStart)} klo ${DateToString.getTime(cartItem.event.eventStart)}` : null;

                return (
                    <>
                        <ListItem dense={true} key={key} sx={{ border: 'solid 0px', borderRadius: '10px', borderColor: 'lightgray' }}>
                            <ListItemAvatar>
                                <Avatar alt="ProductImg" src={cartItem.product.avatarImage} />
                            </ListItemAvatar>
                            <ListItemText
                                sx={{ wordBreak: 'break-word', whiteSpace: 'pre-wrap' }}
                                primary={productName}
                                secondary={
                                    cartItem.product.priceInMinorUnitsIncludingVat / 100
                                    + " € * "
                                    + cartItem.quantity
                                    + " = "
                                    + (cartItem.product.priceInMinorUnitsIncludingVat * cartItem.quantity) / 100
                                    + " €"
                                    + "\n"
                                    + `Sis. Alv ${cartItem.product.vatPercentage} %`
                                }
                            />
                            <IconButton onClick={() => { setQuantity(false) }} ><RemoveCircleIcon sx={{ fontSize: '30px' }} color="error" /></IconButton>
                            <ListItemText sx={{ minWidth: '30px', maxWidth: '30px', display: 'flex', justifyContent: 'center', textAlign: 'center' }} primary={cartItem.quantity}></ListItemText>
                            <IconButton onClick={() => { setQuantity(true) }} edge="end"><AddCircleIcon sx={{ fontSize: '30px' }} color="primary" /></IconButton>

                        </ListItem>

                        {cartItem.discount && (
                            <>
                                <ListItem key={Math.random()} dense={false} sx={{ border: 'solid 0px', borderRadius: '10px', borderColor: 'lightgray' }}>
                                    <ListItemAvatar>
                                        <Avatar alt="ProductImg" src={cartItem.product.avatarImage} />
                                    </ListItemAvatar>
                                    <ListItemText
                                        sx={{ wordBreak: 'break-word', whiteSpace: 'pre-wrap' }}
                                        primary={`Alennus\n${cartItem.product.name}`}
                                        secondary={`${cartItem.discount.discountAmountInMinorUnits / 100} €`}
                                    />
                                    <IconButton onClick={() => {

                                        props.SetShoppingCart(prevValue => {

                                            let copiedCartItems = structuredClone(prevValue);

                                            copiedCartItems.forEach(copiedCartItem => {
                                                if (copiedCartItem.discount?.code === cartItem.discount?.code) {
                                                    copiedCartItem.discount = undefined;
                                                }
                                            });
                                            return copiedCartItems;
                                        })
                                    }} >
                                        <RemoveCircleIcon sx={{ fontSize: '30px' }} color="error" /></IconButton>
                                </ListItem>
                                <Divider />
                            </>

                        )}
                    </>
                )
            })}

        </List>
    );

}

/**
 * 
 * @param props 
 * @returns 
 */
const CheckoutSummary: React.FC<Props> = (props) => {

    let totalSum = 0;

    props.cart.forEach(cartItem => {
        totalSum += cartItem.product.priceInMinorUnitsIncludingVat * cartItem.quantity;
        if(cartItem.discount){
            totalSum -= cartItem.discount.discountAmountInMinorUnits;
        }
    });

    //Prevent negative sum
    if (totalSum < 0) {
        totalSum = 0;
    }

    return (
        <Typography sx={props.sx} variant="h5">{`Yhteensä ${totalSum / 100} €`}</Typography>
    );
}

/**
 * This is used because different products may include identical customer fields, so list only distinct fields.
 * @param cartItems 
 * @returns Distinct list of client fields
 */
const getRequiredInfoFields = (cartItems: ShoppingCartItem[]): ProductSpecificClientFields[] => {

    let productSpesificInfoList: ProductSpecificClientFields[] = [];
    let fieldNamesSet = new Set<string>();

    cartItems.forEach(cartItem => {
        cartItem.product.customerFields?.forEach(customerField => {

            if (!fieldNamesSet.has(customerField.fieldName)) {

                fieldNamesSet.add(customerField.fieldName); 
                productSpesificInfoList.push(customerField); 
            }
        });
    });


    return productSpesificInfoList;
}

export const CheckoutPage: React.FC = () => {

    const [productList, SetProductList] = useState<ShoppingCartItem[]>([]);
    const [pageLoaded, SetPageLoaded] = useState(false);
    const [discountInput, SetDiscountInput] = useState<string>("");
    //const [giftcardCode, SetGiftcardCode] = useState<string | null>(null);
    const [discountError, SetDiscountError] = useState<boolean>(false);
    const [client, SetClient] = useState<ClientModel>(initialCustomerForm);
    const [windowWidth, SetWindowWidth] = useState<number>(window.innerWidth);
    const [error, SetError] = useState<FormErrorState[]>(initialErrorState);
    const [openSnackBar, SetOpenSnackBar] = useState(false);
    const [snackText, SetSnackText] = useState<string>("");
    const [paymentButtonStatus, SetPaymentButtonStatus] = useState<'normal' | 'fetching'>("normal");

    //TODO fix this
    const [checked1,SetChecked1] = useState<boolean>(false);
    const [checked2,SetChecked2] = useState<boolean>(false);
    const [checked3,SetChecked3] = useState<boolean>(false);

    useEffect(() => {

        let storage = localStorage.getItem("shop_cart");

        if (storage) {

            try {

                SetProductList(JSON.parse(storage));
                window.addEventListener('resize', () => { SetWindowWidth(window.innerWidth) });

            } catch (error) {
                console.log("Error. Cant read cart from localstorage.");
            }
        }

        SetPageLoaded(true);

    }, []);

    useEffect(() => {

        //Prevent shopping cart init to set empty value
        if (pageLoaded) {
            localStorage.setItem("shop_cart", JSON.stringify(productList));
        }

    }, [productList]);

    const handleDiscountChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {

        SetDiscountError(false);

        let value: string = event?.target.value;
        value = value?.toUpperCase().trim();

        SetDiscountInput(value);
    }

    const ValidateDiscountCode = async (cart: ShoppingCartItem[], discountCode: string | null) => {

        let productCodes: string[] = [];

        cart.forEach(cartItem => {
            productCodes.push(cartItem.product.code);
        });

        try {
            const discount: Discount = await ApiV1(ApiEndpoint.Discount, Method.POST, true, { productCodes, discountCode });
            SetDiscountError(false);
            SetDiscountInput("");

            SetProductList((prevValue) => {

                let cartItems = structuredClone(prevValue);

                cartItems.forEach(cartItem => {

                    if (cartItem.product.code === discount.targetProductCode) {
                        cartItem.discount = discount;
                    }
                });
                return cartItems;
            });
            

        } catch (error) {
            SetDiscountError(true);
            console.log("Failed to validate the discount code. " + error);

        }
    }

    const handlePaymentButton = async () => {

        //TODO Backend only supports currenly one giftcard or discount code!

        SetPaymentButtonStatus("fetching");

        try {
            await validateClientFields();

            let shoppingCart : ShoppingCartModel = {

                client: client,

                productDetails: productList.map((cartItem) => {

                    return {
                        productCode: cartItem.product.code,
                        quantity: cartItem.quantity,
                        eventCodes: cartItem.event ? [cartItem.event.code] : null,
                    }

                }),

                discountCode: productList[0].discount ? productList[0].discount.code : null,
                giftcardCode: productList[0].giftcard ? productList[0].giftcard.code : null,

            };

            let result: CheckoutResponse = await ApiV1(ApiEndpoint.Checkout, Method.POST, true, shoppingCart);
            
            //If the payment is cleared without payment, redirect to success page
            if (result.href === "success") {
                window.location.href = "/checkout/success";
            }
            else{
                window.location.href = result.href;
            }

        } catch (error) {
            console.log(error);

            if (error instanceof Error) {
                SetSnackText("Virhe: " + error.message);
            }
            else {
                SetSnackText("Tapahtui virhe.");
            }

            SetOpenSnackBar(true);
        }
        finally{
            SetPaymentButtonStatus("normal");
        }

    }

    const validateClientFields = async () => {

        let firstName = client.firstName.trim();
        let lastName = client.lastName.trim();
        let email = client.email.trim();
        let emailConfirmation = client.emailConfirmation.trim();

        //TODO maybe create loop for trimming
        SetClient((prevValue) => ({
            ...prevValue,
            firstName: prevValue.firstName.trim(),
            lastName: prevValue.lastName.trim(),
            email: prevValue.email.trim(),
            emailConfirmation: prevValue.emailConfirmation.trim(),
          }));

        if (!firstName) {
            SetError((prevValue) => ([...prevValue, { fieldName: "firstName", errorText: "Etunimi vaaditaan" }]));
            throw new Error("Anna etunimi.");
            
        }
        if (!lastName) {
            SetError((prevValue) => ([...prevValue, { fieldName: "lastName", errorText: "Sukunimi vaaditaan" }]));
            throw new Error("Anna sukunimi.");
        }
        if (!email) {
            SetError((prevValue) => ([...prevValue, { fieldName: "email", errorText: "Sähköposti vaaditaan" }]));
            throw new Error("Anna sähköposti.");
        }

        if(!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)){
            SetError((prevValue) => ([...prevValue, { fieldName: "email", errorText: "Tarkista sähköposti" }]));
            throw new Error("Tarkista sähköposti.");
        }

        if (email !== emailConfirmation) {
            SetError((prevValue) => ([...prevValue, { fieldName: "emailConfirmation", errorText: "Sähköposti ei täsmää" }]));
            throw new Error("Sähköpostit eivät ole samat.");
        }

        client.additionalInfo.forEach(clientInfo => {

            let value = clientInfo.fieldValue.trim();

            if (!value) {
                SetError((prevValue) => ([...prevValue, { fieldName: clientInfo.fieldName, errorText: `${clientInfo.fieldName} vaaditaan` }]));
                throw new Error(`${clientInfo.fieldName} vaaditaan.`);
            }

            if (value.length >= 100) {
                SetError((prevValue) => ([...prevValue, { fieldName: clientInfo.fieldName, errorText: "Liian pitkä merkkijono" }]));
                throw new Error("Liian pitkä merkkijono.");
            }
        });

        if(!checked1){
            SetError((prevValue) => ([...prevValue, { fieldName: "terms1", errorText: "Vaaditaan!" }]));
            throw new Error("Hyväksy ehdot!");
        }
        if(!checked2){
            SetError((prevValue) => ([...prevValue, { fieldName: "terms2", errorText: "Vaaditaan!" }]));
            throw new Error("Hyväksy ehdot!");
        }

        if(!checked3){
            SetError((prevValue) => ([...prevValue, { fieldName: "terms3", errorText: "Vaaditaan!" }]));
            throw new Error("Hyväksy ehdot!");
        }
        

    }
    
    const handleClose = (
        _event?: React.SyntheticEvent | Event,
        reason?: SnackbarCloseReason,
      ) => {
        if (reason === 'clickaway') {
          return;
        }
    
        SetOpenSnackBar(false);
      };

    return (
        <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%', maxWidth: '900px', justifySelf: 'center' }}>

            <Snackbar
                open={openSnackBar}
                autoHideDuration={3000}
                anchorOrigin={{vertical:'top',horizontal:'right'}}
                onClose={handleClose}>
                <Alert
                    onClose={handleClose}
                    severity="error"
                    variant="filled"
                    sx={{ width: '300px' }}
                >
                    {snackText}
                </Alert>
            </Snackbar>

            <Grid container size={12} sx={{ display: 'flex', justifyContent: 'center', alignItems:'' }}>

                <Grid container size={12} sx={{ marginTop: '30px', marginBottom: '30px', display: 'flex', justifyContent: 'center' }}>
                    <Stepper activeStep={1} alternativeLabel sx={{ width: '700px' }}>
                        {steps.map((label) => (
                            <Step key={label}>
                                <StepLabel>{label}</StepLabel>
                            </Step>
                        ))}
                    </Stepper>
                </Grid>

                {productList.length > 0 ? (
                    <>
                        <Grid size={{ xs: 12, sm: 12, md: 6 }}
                            sx={{
                                maxWidth: '450px',
                                justifyContent: 'center',
                                marginBottom: '30px',
                                paddingRight: windowWidth < 900 ? '0px' : '40px',
                                borderRight: windowWidth < 900 ? '' : 'solid 2px lightgray',
                            }}
                        >

                            <Typography textAlign={"center"} variant="h5">Ostoskori</Typography>

                            <Grid container spacing={1.5} size={12}>

                                <CartList sx={{ width: '100%', padding: '3px', border: 'solid 1px lightgray', borderRadius: '10px' }} cart={productList} SetShoppingCart={SetProductList} />


                                <TextField sx={{width:'250px'}} error={discountError} variant="outlined" value={discountInput} onChange={handleDiscountChange} autoComplete="off" label="Alennuskoodi"></TextField>
                                <Button variant="contained" onClick={() => { ValidateDiscountCode(productList, discountInput) }}>Lisää</Button>


                                <CheckoutSummary sx={{ paddingLeft: '5px', marginTop: '10px', marginBottom: '10px' }} cart={productList} SetShoppingCart={SetProductList} />
                            </Grid>
                        </Grid>


                        <Grid size={{ xs: 12, sm: 12, md: 6 }} sx={{maxWidth:'450px', justifyContent:'center'}}>

                            <Typography textAlign={"center"} variant="h5">Ostajan tiedot</Typography>

                            <Grid container spacing={1.5} size={12} sx={{ display: 'flex', justifyContent: 'center' }}>


                                <TextField
                                    size="small"
                                    type="text"
                                    value={client.firstName}
                                    onChange={(event) => {
                                        SetError(initialErrorState);
                                        SetClient(prevValue => ({ ...prevValue, firstName: event.target.value }))
                                    }}
                                    name="firstName"
                                    autoComplete="off"
                                    sx={{ width: '300px' }}
                                    variant="outlined"
                                    label="Etunimi"
                                    required
                                    error={error.some(error => error.fieldName === "firstName")}
                                    helperText={error.find(error => error.fieldName === "firstName")?.errorText}
                                />
                                <TextField
                                    size="small"
                                    type="text"
                                    value={client.lastName}
                                    onChange={(event) => {
                                        SetError(initialErrorState);
                                        SetClient(prevValue => ({ ...prevValue, lastName: event.target.value }))
                                    }}
                                    name="lastName"
                                    autoComplete="off"
                                    sx={{ width: '300px' }}
                                    variant="outlined"
                                    label="Sukunimi"
                                    required
                                    error={error.some(error => error.fieldName === "lastName")}
                                    helperText={error.find(error => error.fieldName === "lastName")?.errorText}
                                />
                                <TextField
                                    size="small"
                                    type="email"
                                    value={client.email}
                                    onChange={(event) => {
                                        SetError(initialErrorState);
                                        SetClient(prevValue => ({ ...prevValue, email: event.target.value }))
                                    }}
                                    name="email"
                                    autoComplete="email"
                                    sx={{ width: '300px' }}
                                    variant="outlined"
                                    label="Sähköposti"
                                    required
                                    error={error.some(error => error.fieldName === "email")}
                                    helperText={error.find(error => error.fieldName === "email")?.errorText}
                                />
                                <TextField
                                    size="small"
                                    type="text"
                                    value={client.emailConfirmation}
                                    onChange={(event) => {
                                        SetError(initialErrorState);
                                        SetClient(prevValue => ({ ...prevValue, emailConfirmation: event.target.value }));
                                    }}
                                    name="emailConfirmation"
                                    autoComplete="off"
                                    sx={{ width: '300px' }}
                                    variant="outlined"
                                    label="Sähköposti uudestaan"
                                    required
                                    error={error.some(error => error.fieldName === "emailConfirmation")}
                                    helperText={error.find(error => error.fieldName === "emailConfirmation")?.errorText}
                                />

                                {getRequiredInfoFields(productList).map((fieldInfo, index) => {

                                    //To get distinct fields for client object
                                    let clientFieldsSet = new Set<string>();

                                    //Loop client object and add product's wanted info fields to Set. Could be done some where else?
                                    client.additionalInfo.forEach(clientInfo => {
                                        clientFieldsSet.add(clientInfo.fieldName);
                                    });

                                    if (!clientFieldsSet.has(fieldInfo.fieldName)) {
                                        client.additionalInfo.push({ fieldName: fieldInfo.fieldName, fieldType: fieldInfo.fieldType, fieldValue: "" });
                                    }
                                    
                                    return (
                                        <TextField
                                            key={index}
                                            size="small"
                                            sx={{ width: '300px' }}
                                            value={client.additionalInfo.find(info => info.fieldName === fieldInfo.fieldName)?.fieldValue}
                                            onChange={(event) => {
                                                SetError(initialErrorState);
                                                SetClient((prevValue) => {

                                                    //Since spread operator can't make a deep copy, use structured clone
                                                    let clientCopy = structuredClone(prevValue);
                                                    
                                                    clientCopy.additionalInfo.forEach(info => {
                                                        
                                                        if(info.fieldName === event.target.name){
                                                            info.fieldValue = event.target.value;
                                                        }
                                                    });

                                                    return clientCopy;
                                                })
                                            }}
                                            autoComplete="off"
                                            variant="outlined"
                                            label={fieldInfo.fieldName}
                                            name={fieldInfo.fieldName}
                                            required={fieldInfo.isRequired}
                                            error={error.some(error => error.fieldName === fieldInfo.fieldName)}
                                            helperText={error.find(error => error.fieldName === fieldInfo.fieldName)?.errorText}
                                        />
                                    );
                                })}

                                <FormControl error={error.some(error => error.fieldName.includes("terms"))}>
                                    <FormGroup sx={{ width: '300px' }}>
                                        <FormControlLabel required control={<Checkbox checked={checked1} onChange={(event) => { SetChecked1(event.target.checked); SetError([]) }} />} label={<span> Hyväksyn <Link href="https://klu.fi/tietosuoja.pdf" target="_blank" rel="noreferrer">tietosuojaselosteen</Link></span>} />
                                        <FormHelperText>{error.find(error => error.fieldName === "terms1")?.errorText}</FormHelperText>
                                    </FormGroup>
                                    <FormGroup>
                                        <FormControlLabel required control={<Checkbox checked={checked2} onChange={(event) => { SetChecked2(event.target.checked); SetError([]) }} />} label={<span> Hyväksyn <Link href="https://klu.fi/tandemehdot.pdf" target="_blank" rel="noreferrer">toimitus- ja peruutusehdot</Link></span>} />
                                        <FormHelperText>{error.find(error => error.fieldName === "terms2")?.errorText}</FormHelperText>
                                    </FormGroup>
                                    <FormGroup>
                                        <FormControlLabel required control={<Checkbox checked={checked3} onChange={(event) => { SetChecked3(event.target.checked); SetError([]) }} />} label={<span> Hyväksyn <Link href="https://www.paytrail.com/kuluttaja/tietoa-maksamisesta" target="_blank" rel="noreferrer">maksuehdot</Link></span>} />
                                        <FormHelperText>{error.find(error => error.fieldName === "terms3")?.errorText}</FormHelperText>
                                    </FormGroup>
                                </FormControl>

                                <Typography sx={{ width: '300px', textAlign: 'start' }} variant="body2">* Pakollinen kenttä</Typography>

                                {paymentButtonStatus === "normal" ? (
                                    <>
                                    <Button sx={{ width: '200px' }} color="success" variant="contained" onClick={handlePaymentButton}>Siirry maksamaan</Button>
                                    </>
                                ):(
                                    <>
                                    <Box sx={{display:'flex', width:'100%', justifyContent:'center'}}>
                                        <CircularProgress></CircularProgress>
                                    </Box>
                                    </>
                                )}
                                
                                <Typography sx={{ width: '360px', textAlign: 'center' }} variant="h6">Maksa turvallisesti paytrailin kautta!</Typography>
                            </Grid>

                        </Grid>

                        <Grid container size={12} spacing={3} sx={{display:'flex', justifyContent:'center', marginTop:'50px'}}>
                        <img style={{width:'100%',maxWidth:'400px'}} src={PaytrailImage}></img>
                        <Typography sx={{width:'100%'}} textAlign={"center"} variant='h5'>Kuopion Laskuvarjourheilijat Ry {(new Date).getFullYear()}</Typography>
                        </Grid>
                        
                    </>

                ) : (
                    <>
                        <Grid size={{ xs: 12 }} sx={{ display: 'flex', justifyContent: 'center' }}>
                            <Typography variant="h6">Ostoskori on tyhjä</Typography>
                        </Grid>
                    </>
                )}


            </Grid>
        </Box>
    );

}