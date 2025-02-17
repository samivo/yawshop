
/**
 * Shoppingcart model for backend
 */
export interface ShoppingCartModel{

    client: ClientModel,
    productDetails : ProductInCart[],
    discountCode: string | null,
    giftcardCode : string | null

}

export interface ProductInCart{
    productCode: string,
    eventCodes : string[] | null,
    quantity : number 
}

export interface ClientModel{
    code: string | null,
    firstName: string,
    lastName: string,
    email: string,
    emailConfirmation: string,
    additionalInfo: AdditionalClientInfo[],
}

export interface AdditionalClientInfo{
    fieldName: string,
    fieldValue: string,
    fieldType: CustomerFieldType,
}

export enum CustomerFieldType{
    Text = 0,
    Integer = 1,
    Decimal = 2,
    DateTime = 3,
    Boolean = 4,
    Agreement = 5
}