export interface ProductModel {
    id: number;
    code: string;
    name: string;
    isActive: boolean;
    isVisibleToPublic: boolean;
    maxQuantityPerPurchase: number | null;
    quantityTotal: number | null;
    quantityUsed: number ;
    quantityLeft: number | null;
    priceInMinorUnitsIncludingVat: number;
    vatPercentage: number;
    shortDescription: string | null;
    descriptionOrInnerHtml: string | null;
    avatarImage: string | null;
    internalComment: string | null;
    availableFrom: Date;
    availableTo: Date | null;
    productType: ProductType;
    customerFields: ProductSpecificClientFields[] | null;
    productGroupId: number | null;
    giftcardTargetProductCode: string;
    giftcardPeriodInDays: number;
    createdAt: Date | null;
    modifiedAt: Date | null;
    modifier: string | null;
}

export interface ProductSpecificClientFields {
    id: number ;
    productModelId: number;
    fieldName: string;
    isRequired: boolean;
    href: string | null;
    fieldType: CustomerFieldType;
}

export enum ProductType {
    Virtual = 0,
    Giftcard = 1,
    Event = 2,
  }

export enum CustomerFieldType {
    Text = 0,
    Integer = 1,
    Decimal = 2,
    DateTime = 3,
    Boolean = 4,
    Agreement = 5,
  }