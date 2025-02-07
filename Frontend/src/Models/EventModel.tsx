import { EventStatus } from "../Utilities/EventModelPublic";

export interface EventModel {
  code: string;
  productCode: string;
  registrationsQuantityTotal: number | null;
  registrationsQuantityUsed: number;
  registrationsLeft: number | null;
  eventStart: Date;
  eventEnd: Date;
  hoursBeforeEventUnavailable: number;
  status: EventStatus;
  clientCodes: string[] | null;
}