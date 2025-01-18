
export interface EventModelPublicModel {
    code: string,
    productCode: string,
    registrationsLeft: number | null,
    eventStart: Date,
    eventEnd: Date,
    hoursBeforeEventUnavailable: number,
    status: EventStatus,
}

export enum EventStatus {
    Available = 0,
    Full = 1,
    Cancelled = 2,
    Finished = 3,
    Expired = 4,
}