import { Box, Button, Grid2, TextField } from '@mui/material'
import React, { ChangeEvent,  useState } from 'react'
import { postCredentials } from '../../../Utilities/PostCredentials';
import { useNavigate } from 'react-router-dom';

export interface LoginForm {
    email: string;
    password: string;
}

const LoginPage: React.FC = () => {

    const navigation = useNavigate();

    const [loginForm, SetLoginForm] = useState<LoginForm>({
        email: "",
        password: "",
    });

    const [errors, setErrors] = useState({
        email: "",
        password: "",
      });


    const HandleChange = ( event : ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {

        SetLoginForm((prevData) => ({ ...prevData, [event.target.id]: event.target.value }));

    }

    const HandleSubmit = async () => {

        const newErrors = { email: '', password: '' };

        if (!loginForm.password.trim()) {
            newErrors.password = "Salasana on pakollinen";
        }

        if (!loginForm.email.trim()) {
            newErrors.email = 'Sähköposti on pakollinen';
          } else if (!/\S+@\S+\.\S+/.test(loginForm.email)) {
            newErrors.email = 'Tarkista sähköposti';
          }

        setErrors(newErrors);

        if (!newErrors.email && !newErrors.password) {

            const success = await postCredentials(loginForm);

            if(success){

                navigation("/dashboard");

            }
            else{
                setErrors({
                    email:"Tarkista sähköposti",
                    password:"Tarkista salasana"
                });
            }
            
        }

    }

    return (
        <Box sx={{
            display: 'flex',
            width: '100vw',
            height: '100vh',
            alignItems: 'center',
            justifyContent: 'center'
        }}>

            <Grid2 container size={12} spacing={2}>

                <Grid2 size={12} sx={{ display: 'flex', justifyContent: 'center' }}>
                    <TextField
                        sx={{ width: '300px' }}
                        autoComplete='off'
                        required 
                        id="email"
                        label="Sähköposti"
                        variant="outlined"
                        type='email'
                        error={!!errors.email}
                        helperText={errors.email}
                        onChange={HandleChange} />

                </Grid2>

                <Grid2 size={12} sx={{ display: 'flex', justifyContent: 'center' }}>
                    <TextField
                        sx={{ width: '300px' }}
                        autoComplete='off'
                        required 
                        id="password"
                        label="Salasana"
                        variant="outlined"
                        type='password'
                        error={!!errors.password}
                        helperText={errors.password}
                        onChange={HandleChange}
                        />
                </Grid2>

                <Grid2 size={12} sx={{ display: 'flex', justifyContent: 'center' }}>
                    <Button variant="contained" onClick={HandleSubmit}>Kirjaudu</Button>
                </Grid2>

            

            </Grid2>


        </Box>
    )
}

export default LoginPage