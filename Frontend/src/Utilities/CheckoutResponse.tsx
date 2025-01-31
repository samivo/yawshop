export interface CheckoutResponse {
  reference: string;
  totalAmount: number;
  clientId: number;
  products: {
    unitPrice: number;
    units: number;
    vatPercentage: number;
    productCode: string;
    productName: string;
    eventCode: string | null;
  }[];
  transactionId: string | null;
  paymentStatus: PaymentStatus;
  paymentMethod: string | null;
  internalComment: string | null;
  createdAt: string;
  updatetAt: string;
  modifierName: string | null;
  hash: string | null;
}

export enum PaymentStatus {
  Initialized = 0,
  New = 1,
  Ok = 2,
  Fail = 3,
  Cancelled = 4,
  Pending = 5,
  Delayed = 6,
}
