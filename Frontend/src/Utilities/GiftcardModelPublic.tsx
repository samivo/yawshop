export interface GiftcardModelPublic {
    code: string; 
    name: string; 
    valueLeftInMinorUnits: number; 
    purchaseDate: Date; 
    expireDate: Date; 
    usedDate: Date | null;
}