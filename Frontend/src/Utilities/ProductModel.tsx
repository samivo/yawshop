export default interface ProductModel {
  id: number;
  code: string;
  name: string;
  isActive?: boolean;
  isVisibleToPublic?: boolean;
  maxQuantityPerPurchase?: number | null;
  quantityTotal?: number | null;
  quantityUsed?: number;
  quantityLeft?: number | null;
  priceInMinorUnitsIncludingVat: number;
  vatPercentage: number;
  shortDescription: string;
  descriptionOrInnerHtml: string;
  avatarImage?: string;
  internalComment?: string;
  availableFrom?: Date;
  availableTo?: Date | null;
  productType: ProductType;
  customerFields?: ProductSpesificClientFields[] | null;
  productGroupId?: number | null;
  giftcardTargetProductCode: string;
  giftcardPeriodInDays: number;
  createdAt?: Date;
  modifiedAt?: Date;
  modifier?: string | null;
}

export interface ProductSpesificClientFields {
  fieldName: string;
  fieldNamePublic: string;
  isRequired: boolean;
  href?: string | null;
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