import { Box, Grid2, Link, Typography } from "@mui/material";
import { useEffect, useState } from "react";



export const PaymentCancel: React.FC = () => {

    const [timeToRedirect, SetTimeToRedirect] = useState(30);

    //Clears the shopping cart
    localStorage.clear();

    useEffect(() => {

        const interval = setInterval(() => {

            if (timeToRedirect > 0) {
                SetTimeToRedirect((prevTime) => {
                    if (prevTime > 0) {
                        return prevTime - 1;
                    }
                    else {
                        window.location.replace("https://klu.fi");
                        clearInterval(interval);
                        return 0;
                    }
                });
            }
            
        }, 1000);

        return () => {
            clearInterval(interval)
        };

    },[]);

    

    return (
        <Box sx={{ display: 'flex', width: '100%', justifyContent: 'center' }}>
            <Grid2 container size={12} sx={{maxWidth:'600px', marginTop:'150px'}}>
                <Grid2 size={12}>
                <Typography textAlign={"center"} variant="h4">Stuntti seis!</Typography>
                <Typography textAlign={"center"} variant="h4">Maksu keskeytetty. </Typography>
                <Typography textAlign={"center"} variant="h6">Jos tämä ei ollut toivottua, ota meihin yhteyttä!</Typography>
                <Typography textAlign={"center"} variant="body1">Siirrämme sinut takaisin klu.fi etusivulle {timeToRedirect} sekunnin kuluttua.</Typography>
                <Typography textAlign={"center"} variant="body1">Tai siirry suoraan <Link href="https://klu.fi"> klu.fi</Link></Typography>
                </Grid2>
            </Grid2>
        </Box>
    );
}