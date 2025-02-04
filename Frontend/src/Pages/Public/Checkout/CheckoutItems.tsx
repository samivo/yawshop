import { Avatar, Divider, IconButton, List, ListItem, ListItemAvatar, ListItemText, SxProps } from "@mui/material";
import { ShoppingCartItem } from "../../../Utilities/ShoppingCartItem";
import { ProductType } from "../../../Models/ProductModel";
import { DateToString } from "../../../Utilities/DateToString";
import RemoveCircleIcon from '@mui/icons-material/RemoveCircle';
import AddCircleIcon from '@mui/icons-material/AddCircle';

interface Props {

    cart: ShoppingCartItem[],
    SetShoppingCart: React.Dispatch<React.SetStateAction<ShoppingCartItem[]>>
    sx? : SxProps
}

export const CartList: React.FC<Props> = (props) => {

    return (
        <List sx={props.sx}>

            {props.cart.map((cartItem, key) => {

                const setQuantity = (increase: boolean) => {

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
                    </>
                )
            })}

        </List>
    );

}