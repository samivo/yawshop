import { ProductType, ProductSpecificClientFields } from "./ProductModel";

export interface ProductModelPublic {
    code: string;
    name: string;
    maxQuantityPerPurchase?: number;
    quantityLeft?: number;
    priceInMinorUnitsIncludingVat: number;
    vatPercentage: number;
    shortDescription: string;
    descriptionOrInnerHtml: string;
    avatarImage?: string;
    customerFields?: ProductSpecificClientFields[];
    productGroupId?: number;
    giftcardTargetProductCode: string;
    giftcardPeriodInDays: number;
    productType: ProductType;
}
