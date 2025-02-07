import { Discount } from "./DiscountModel";
import { EventModelPublic } from "./EventModelPublic";
import { Giftcard } from "./GiftcardModel";
import { ProductModelPublic } from "../Models/ProductModelPublic";

export interface ShoppingCartItem {

    product: ProductModelPublic;
    event?: EventModelPublic | null;
    quantity: number;
    discount?: Discount;
    giftcard?: Giftcard;
}