import { ClientModel } from "../Utilities/ShoppingCartModel";

export interface CheckoutModel {
    id:string;
    reference: string;
    totalAmount: number;
    client: ClientModel;
    products: CheckoutItem[];
    transactionId: string | null;
    paymentStatus: PaymentStatus;
    paymentMethod: string | null;
    createdAt: string; // ISO string format for dates
    updatedAt: string; // ISO string format for dates
    internalComment: string | null;
    modifierNme: string | null;
}

export interface CheckoutItem {
    unitPrice: number;
    units: number;
    vatPercentage: number;
    productCode: string;
    productName: string;
    eventCode: string | null;
    discountCode: string | null;
    giftcardCode: string | null;
}

export enum PaymentStatus {
    Initialized,
    New,
    Ok,
    Fail,
    Cancelled,
    Pending,
    Delayed
}