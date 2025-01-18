import { useState, useEffect } from 'react';
import { ApiEndpoint, ApiV1, Method } from './ApiFetch';

export function useAuth() {

  const [authState, setAuthState] = useState<'pending' | 'authenticated' | 'unauthenticated'>('pending');

  useEffect(() => {

    try {
      ApiV1(ApiEndpoint.CheckAuth, Method.POST, false, {});
      setAuthState('authenticated');

    } catch (error) {
      setAuthState('unauthenticated');
    }

  }, []);

  return authState;
}
