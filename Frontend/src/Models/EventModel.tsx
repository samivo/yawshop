import { ClientModel } from "../Utilities/ShoppingCartModel";

export interface EventModel {
  code: string;
  productCode: string;
  eventStart: Date;
  eventEnd: Date;
  hoursBeforeEventUnavailable: number;
  isVisible: boolean;
  isAvailable: boolean;
  clientCode: string | null;
  client: ClientModel | null;
}