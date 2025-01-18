import { LoginForm } from "../Pages/Management/Login/LoginPage";


export async function postCredentials(loginForm: LoginForm): Promise<boolean> {
    
    try {
        const response = await fetch('/api/v1/auth/login/?useCookies=true&useSessionCookies=false', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(loginForm),
        });

        if (!response.ok) {
            return false;
        }

        return true;

    } catch (error) {
        return false;
    }
}
