import { Discount } from "./DiscountModel";
import { EventModelPublicModel } from "./EventModelPublicModel";
import { Giftcard } from "./GiftcardModel";
import { ProductModelPublic } from "./ProductModelPublic";

export interface ShoppingCartItem {

    product: ProductModelPublic;
    event?: EventModelPublicModel | null;
    quantity: number;
    discount?: Discount;
    giftcard?: Giftcard;
}