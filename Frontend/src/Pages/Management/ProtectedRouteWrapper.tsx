import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Box, CircularProgress } from '@mui/material';
import { ApiEndpoint, ApiV1, Method } from '../../Utilities/ApiFetch';

function ProtectedRoute({ children }: { children: React.ReactElement }) {

  const [authState, setAuthState] = useState<'pending' | 'authenticated' | 'unauthenticated'>('pending');
  const navigation = useNavigate();

  useEffect( () => {
    checkAuth();

  }, []);

  const checkAuth = async () => {


    try {
      await ApiV1(ApiEndpoint.CheckAuth, Method.GET, false);
      setAuthState("authenticated")

    } catch (error) {
      setAuthState("unauthenticated");
    }
  }


  if (authState === 'pending') {
   
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
    return children;
  }
  
  if(authState === "unauthenticated"){
    navigation("/login");
  }
}

export default ProtectedRoute;
