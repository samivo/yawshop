import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Box, CircularProgress } from '@mui/material';
import { ApiEndpoint, ApiV1, Method } from '../../Utilities/ApiFetch';

function ProtectedRoute({ children }: { children: React.ReactElement }) {

  const [authState, setAuthState] = useState<'pending' | 'authenticated' | 'unauthenticated'>('pending');
  const navigation = useNavigate();

  console.log(`Protected route. Status: ${authState}`);
  
  useEffect( () => {
    console.log(`use Effect. Status: ${authState}`);
    checkAuth();

  }, []);

  useEffect(()=>{
    console.log(`Auth state useffect. Status: ${authState}`);
  },[authState]);

  const checkAuth = async () => {


    try {
      console.log(`Start api fetch. Status: ${authState}`);
      await ApiV1(ApiEndpoint.CheckAuth, Method.GET, false);
      setAuthState("authenticated")

    } catch (error) {
      console.log(`Fetch error. Status: ${authState}`);
      setAuthState("unauthenticated");
    }
  }


  if (authState === 'pending') {
   
    console.log(`Auth state pending. Status: ${authState}`);
    return (
      <Box sx={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        width: '100vw',
        height: '100vh',
      }}>
        <CircularProgress />
      </Box>

    );
  }

  if(authState === "authenticated"){
    console.log(`Auth state authenticated. Status: ${authState}`);
    return children;
  }
  
  if(authState === "unauthenticated"){
    console.log(`Auth state unauthenticated. Return /login. Status: ${authState}`);
    navigation("/login");
  }
}

export default ProtectedRoute;
