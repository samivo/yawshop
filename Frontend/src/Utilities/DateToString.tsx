
/** 
* Because js Date to isostring causes problems with timezones use this class to create date and time strings from date
*/

export class DateToString {

    private constructor() {
        // Prevent instantiation
    }

    /**
     * 
     * @param inputDate 
     * @returns "Date string in format DD.MM.YYY"
     */
    static getDate = (inputDate: Date | string | undefined): string => {

        let date: Date;

        if(!inputDate){
            return "";
        }


        try {

            if (typeof inputDate === "string") {
                date = new Date(inputDate);
            }
            else {
                date = inputDate;
            }

            return (date.getDate().toString().padStart(2, "0") + "." + (date.getMonth() + 1) + "." + date.getFullYear());

        } catch (error) {
            throw error;
        }
    }

    /**
     * 
     * @param inputDate 
     * @returns "Time string in format HH:MM"
     */

    static getTime = (inputDate: Date | string): string => {

        let date: Date;


        try {

            if (typeof inputDate === "string") {
                date = new Date(inputDate);
            }
            else {
                date = inputDate;
            }

            return (date.getHours().toString().padStart(2,"0")+":"+date.getMinutes().toString().padStart(2,"0"));

        } catch (error) {
            throw error;
        }
    }
}