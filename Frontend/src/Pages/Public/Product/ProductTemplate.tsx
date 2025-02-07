import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { Avatar, Badge, Box, Button, List, ListItemButton, ListItemIcon, ListItemText, Paper, Step, StepLabel, Stepper, Typography } from '@mui/material';
import Grid from '@mui/material/Grid2';
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import { ProductModelPublic } from '../../../Models/ProductModelPublic';
import ArrowForwardIcon from '@mui/icons-material/ArrowForward';
import { DateCalendar, PickersDay, PickersDayProps } from '@mui/x-date-pickers';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFnsV3';
import { fi as locale } from 'date-fns/locale';
import { EventModelPublic, EventStatus } from '../../../Utilities/EventModelPublic';
import { ApiEndpoint, ApiV1, Method } from '../../../Utilities/ApiFetch';
import { isNumber } from '@mui/x-data-grid/internals';
import ScheduleIcon from '@mui/icons-material/Schedule';
import { DateToString } from '../../../Utilities/DateToString';
import { ShoppingCartItem } from '../../../Utilities/ShoppingCartItem';
import { useNavigate } from 'react-router-dom';
import { ProductType } from '../../../Models/ProductModel';

const steps = [
    'Valitse tuote',
    'Kassa - Täytä tiedot',
    'Maksa',
];

const showFullEvents = true;

const ProductTemplate: React.FC = () => {

    const { code } = useParams<{ code: string }>();
    let navigate = useNavigate();

    const [products, SetProducts] = useState<ProductModelPublic[]>();
    const [product, SetProduct] = useState<ProductModelPublic>();
    const [events, SetEvents] = useState<EventModelPublic[]>();
    const [selectedDate, SetSelectedDate] = useState<Date>(new Date);
    const [eventsInSelectedDate, SetEventsInSelectedDate] = useState<EventModelPublic[] | undefined>();
    const [selectedEvent, SetSelectedEvent] = useState<EventModelPublic | null>(null);

    const fetch = async () => {

        let prods: ProductModelPublic[] = await ApiV1(ApiEndpoint.Product, Method.GET, true);
        let events: EventModelPublic[] = await ApiV1(ApiEndpoint.Event, Method.GET, true);

        //Get upcoming events for this product
        events = events.filter(event => event.productCode == code && (new Date(event.eventStart)) > new Date());

        SetProducts(prods);
        SetEvents(events);

        //Prechoose the first available event
        if (events.length > 0) {

            //Sort events by start date
            events = events.sort((a, b) => (new Date(a.eventStart)).getTime() - (new Date(b.eventStart)).getTime());

            let event = events.find(event => event.status === EventStatus.Available);

            if (event) {
                SetSelectedDate(new Date(event.eventStart));
                SetSelectedEvent(event);
            }
        }

    };

    useEffect(() => {
        fetch();
    }, []);


    useEffect(() => {

        var prod = products?.find(product => product.code === code);
        SetProduct(prod);

    }, [products]);

    useEffect(() => {

        let targetDate = selectedDate.getFullYear() + "-" + (selectedDate.getMonth() + 1).toString().padStart(2, '0') + "-" + selectedDate.getDate().toString().padStart(2, '0');

        let targetEvents = events?.filter(event => {

            if ((new Date(event.eventStart).toISOString().substring(0, 10)) === targetDate) {
                return true;
            }

        });

        if (targetEvents?.length != undefined && targetEvents?.length > 0) {
            SetEventsInSelectedDate(targetEvents);
        }
        else {
            SetEventsInSelectedDate(undefined);
        }


    }, [selectedDate]);

    const CustomDateSlot = (props: PickersDayProps<Date>) => {

        let eventsLeftCount: number | string = 0;
        let totalEventsCount: number = 0;

        events?.forEach(event => {

            if (DateToString.getDate(event.eventStart) === DateToString.getDate(props.day)) {

                totalEventsCount++;

                if (event.registrationsLeft != null && event.registrationsLeft != undefined && isNumber(eventsLeftCount)) {

                    if (event.status === EventStatus.Available) {
                        eventsLeftCount += event.registrationsLeft;
                    }

                }
                else {
                    eventsLeftCount = 11;
                }

            }

        });

        if (totalEventsCount > 0 && eventsLeftCount == 0 && showFullEvents) {
            eventsLeftCount = "0";
        }

        return (
            <>
                <Badge overlap='circular' max={10} badgeContent={props.outsideCurrentMonth ? null : eventsLeftCount} color={eventsLeftCount === "0" ? "error" : "success"}>
                    <PickersDay sx={{ zIndex: '2' }} {...props} showDaysOutsideCurrentMonth={false} />
                </Badge>

            </>
        );
    }

    const EventList = () => {

        return (
            <List dense={true}
            >

                {eventsInSelectedDate?.map((event: EventModelPublic, key: number) => {

                    return (
                        <ListItemButton
                            sx={{
                                border: 'solid 1px',
                                maxWidth: '100%',
                                borderRadius: '10px',
                                borderColor: event.status === EventStatus.Available ? "green" : "red",
                                marginBottom: '5px',
                                //backgroundColor: event.code === selectedEvent?.code ? "" : "",
                                borderWidth: event.code === selectedEvent?.code ? "2px" : "1px",
                            }}
                            disabled={event.status === EventStatus.Available ? false : true}
                            onClick={() => { SetSelectedEvent(event) }}
                            key={key}
                        >

                            <ListItemIcon sx={{ minWidth: '38px' }} >
                                <ScheduleIcon></ScheduleIcon>
                            </ListItemIcon>

                            <ListItemText
                                primary={DateToString.getDate(event.eventStart)}
                                secondary={DateToString.getTime(event.eventStart)}
                            >

                            </ListItemText>

                            <ListItemIcon sx={{ display: 'flex', justifyContent: 'center' }}>
                                <ArrowForwardIcon></ArrowForwardIcon>
                            </ListItemIcon>

                            <ListItemText
                                primary={DateToString.getDate(event.eventEnd)}
                                secondary={DateToString.getTime(event.eventEnd)}
                            >

                            </ListItemText>

                            <ListItemText sx={{ textAlign: 'center', marginLeft: '10px' }} primary="Tilaa" secondary={event.registrationsLeft != null ? event.registrationsLeft : "On"}></ListItemText>

                        </ListItemButton>
                    );
                })}


            </List>
        );
    }

    const checkOut = () => {

        if (!product) {
            throw new Error("Checkout button clicked but there is no product?");

        }

        if (product?.productType === ProductType.Event) {

            if (!selectedEvent || !product) {
                throw "Checkout button clicked without product or event selected?";
            }

            if (selectedEvent.status !== EventStatus.Available) {
                throw "Checkout button clicked with event that is not available";
            }
            if (selectedEvent.registrationsLeft && selectedEvent.registrationsLeft <= 0) {
                throw "Checkout button clicked but event has no registrations left";
            }
        }
        else {

            if (product?.quantityLeft && product.quantityLeft <= 0) {
                throw "Checkout button clicked but product has no quantity left";
            }
        }

        try {

            //TODO: Shop is desinged to use shopping cart. Currently checkout one item directly.

            let shoppingCart: ShoppingCartItem[] = [];

            shoppingCart.push({ product: product, event: selectedEvent, quantity: 1 });

            localStorage.setItem("shop_cart", JSON.stringify(shoppingCart));

            navigate("/checkout");

        } catch (error) {
            throw error;

        }
    }

    return (

        <>
            {product ? (

                <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%' }}>
                    <Paper elevation={0} sx={{ width: '1100px', maxWidth: '100%' }}>
                        <Grid container>

                            <Grid container size={12} sx={{ marginTop: '30px', marginBottom: '30px', display: 'flex', justifyContent: 'center' }}>
                                <Stepper activeStep={0} alternativeLabel sx={{ width: '700px' }}>
                                    {steps.map((label) => (
                                        <Step key={label}>
                                            <StepLabel>{label}</StepLabel>
                                        </Step>
                                    ))}
                                </Stepper>
                            </Grid>

                            <Grid size={{ xs: 12, md: 7 }} sx={{ padding: '10px' }}>
                                <Grid size={12} sx={{ display: 'flex', justifyContent: 'center' }}>
                                    <Avatar alt="productAvatar" sx={{ width: 300, height: 300, borderRadius: '50%' }} src={product.avatarImage} />
                                </Grid>
                                <Grid size={12} >
                                    <div style={{ width: '100%', overflow: 'hidden' }} dangerouslySetInnerHTML={{ __html: product.descriptionOrInnerHtml }} />
                                </Grid>
                            </Grid>


                            <Grid container size={{ xs: 12, md: 5 }} sx={{ display: 'flex', justifyContent: 'center' }} >
                                <Paper elevation={0} sx={{ padding: '10px' }}>

                                    <Grid container size={12} spacing={1} >

                                        <Grid size={12} sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', textAlign: 'center' }}>
                                            <Typography id="productName" variant='h5' >{product.name}</Typography>
                                        </Grid>

                                        <Grid size={12} sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                                            <Typography id="productPrice" variant='h5'>{product.priceInMinorUnitsIncludingVat / 100} €</Typography>
                                            <Typography id="ProductVat" variant='body1'>(Sis. Alv {product.vatPercentage} %)</Typography>
                                        </Grid>

                                        {product.productType !== ProductType.Event && (
                                            <>
                                                {product.quantityLeft && (
                                                    <>
                                                        <Grid size={12} sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                                                            <Typography variant='h6'>Jäljellä: {product.quantityLeft}</Typography>
                                                        </Grid>
                                                    </>
                                                )}

                                                {product.productType === ProductType.Giftcard && (
                                                    <>
                                                        <Grid size={12} sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                                                            <Typography variant='h6'>Voimassa {product.giftcardPeriodInDays} päivää</Typography>
                                                        </Grid>
                                                    </>
                                                )}

                                                <Grid size={12} sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', marginTop: '10px', marginBottom: '20px' }}>
                                                    <Button endIcon={<ArrowForwardIcon />} onClick={checkOut} variant="contained">Kassalle</Button>
                                                </Grid>
                                            </>
                                        )}

                                        {product.productType === ProductType.Event && (
                                            <>

                                                <Grid size={12} sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}>

                                                    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={locale}>
                                                        <DateCalendar
                                                            disablePast={true}
                                                            value={selectedDate}
                                                            onChange={(newValue) => {
                                                                SetSelectedDate(newValue);
                                                                SetSelectedEvent(null);
                                                            }}
                                                            slots={{ day: CustomDateSlot }}
                                                        />
                                                    </LocalizationProvider>

                                                </Grid>

                                                {eventsInSelectedDate ? (
                                                    <>
                                                        <Grid size={12} sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                                                            <Typography variant='h5'>Valitse tapahtuma</Typography>
                                                        </Grid>

                                                        <Grid size={12} sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                                                            <EventList />
                                                        </Grid>

                                                    </>

                                                ) : (
                                                    <Grid size={12} sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                                                        <Typography variant='h5'>Ei tapahtumia {DateToString.getDate(selectedDate)}</Typography>
                                                    </Grid>
                                                )}

                                                {selectedEvent && (
                                                    <Grid container size={12} sx={{}} >

                                                        <Grid size={12} sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', marginTop: '10px' }}>
                                                            <Typography variant='h6' >Valittu tapahtuma</Typography>
                                                        </Grid>

                                                        <Grid size={12} sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                                                            <Typography variant='body1' >{DateToString.getDate(selectedEvent.eventStart)} klo {DateToString.getTime(selectedEvent.eventStart)}</Typography>
                                                        </Grid>

                                                        <Grid size={12} sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', marginTop: '10px', marginBottom: '20px' }}>
                                                            <Button size='large' endIcon={<ArrowForwardIcon />} variant="contained" onClick={checkOut}>Kassalle</Button>
                                                        </Grid>

                                                    </Grid>
                                                )}

                                            </>
                                        )}


                                    </Grid>
                                </Paper>
                            </Grid>

                            <Grid size={12} sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', marginTop: '20px', marginBottom: '20px' }}>
                                <Typography sx={{ width: '100%' }} textAlign={"center"} variant='h5'>Kuopion Laskuvarjourheilijat Ry {(new Date).getFullYear()}</Typography>
                            </Grid>

                        </Grid>
                    </Paper>
                </Box>

            ) : (
                null
            )}

        </>
    );
};

export default ProductTemplate;
