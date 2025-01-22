import { Box, Grid2, Typography } from "@mui/material";
import { useParams } from "react-router-dom";
import lahjakortti from "./Lahjakortti_v3.svg";
import { useEffect, useState } from "react";
import { GiftcardModelPublic } from "../../../Utilities/GiftcardModelPublic";
import { ApiEndpoint, ApiV1, Method } from "../../../Utilities/ApiFetch";
import { DateToString } from "../../../Utilities/DateToString";


export const GiftcardInfo: React.FC = () => {

    const { code } = useParams<{ code: string }>();
    const [giftcardSvg, SetGiftcardSvg] = useState<string | null>(null);

    useEffect(() => {

        const fetchData = async () => {
            try {
                const response: GiftcardModelPublic = await ApiV1(ApiEndpoint.Giftcard, Method.GET, true, null, `/${code}`);

                const svg = await fetch(lahjakortti);
                let svgText = await svg.text();

                const modifiedGiftcard = modifyGiftcard(response, svgText);
                SetGiftcardSvg(modifiedGiftcard);


            } catch (error) {
                console.error("Error fetching giftcard:", error);
            }
        };

        fetchData();

    }, [code]);


    const modifyGiftcard = (giftcardInfo: GiftcardModelPublic, giftcardSvg: string): string => {

        let card = giftcardSvg;

        let expireDate = DateToString.getDate(giftcardInfo.expireDate);
        
        let usedDate = giftcardInfo.usedDate ?  DateToString.getDate(giftcardInfo.usedDate) : "";
        
        card = card.replace(/{{empty}}/g,"",);
        card = card.replace(/{{giftcardName}}/g,giftcardInfo.name);
        card = card.replace(/{{expires}}/g, expireDate);
        card = card.replace(/{{code}}/g, giftcardInfo.code);
        card = card.replace(/{{used}}/g, usedDate);

        return card;
    }

    if (giftcardSvg) {
        return (
            <Box
                sx={{
                    display: "flex",
                    width: "100%",
                    justifyContent: "center",
                    alignItems: "start",
                    marginTop: "50px",
                }}
            >
                <div
                    dangerouslySetInnerHTML={{ __html: giftcardSvg }}
                    style={{ maxWidth: "100%", width: "800px" }}
                ></div>
            </Box>
        );
    }
    else {
        return (
            <Box
                sx={{
                    display: "flex",
                    width: "500px",
                    maxWidth:'100%',
                    justifyContent: "center",
                    alignItems: "start",
                    marginTop: "50px",
                    justifySelf:'center'
                }}
            >
                <Grid2 container size={12} spacing={3} sx={{padding:'10px'}}>
                    <Typography variant="h5">Mikäli olet ostanut lahjakortin, ei hätää!</Typography>
                    <Typography variant="h5">Päivitämme verkkokauppaa ja lahjakorttisi tulee tänne piakkoin.</Typography>
                </Grid2>


            </Box>
        );

    }

}