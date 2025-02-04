import { SxProps, Typography } from "@mui/material";
import { ShoppingCartItem } from "../../../Utilities/ShoppingCartItem";

interface Props {

    cart: ShoppingCartItem[],
    sx? : SxProps
}

export const CheckoutSummary: React.FC<Props> = (props) => {

    let totalSum = 0;

    props.cart.forEach(cartItem => {
        totalSum += cartItem.product.priceInMinorUnitsIncludingVat * cartItem.quantity;
    });

    //Prevent negative sum
    if (totalSum < 0) {
        totalSum = 0;
    }

    return (
        <Typography sx={props.sx} variant="h5">{`Yhteensä ${totalSum / 100} €`}</Typography>
    );
}