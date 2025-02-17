
export interface EventModelPublic {
    code: string,
    productCode: string,
    eventStart: Date,
    eventEnd: Date,
    hoursBeforeEventUnavailable: number,
    isAvailable:boolean;
}