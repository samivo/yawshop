
export async function ApiV1(endpoint: ApiEndpoint, method: Method, publicApi: boolean, fetchBody?: any, queryParameter?: string): Promise<any> {

    try {

        const response = await fetch(endpoint + (publicApi ? "/public" : "") + (queryParameter? queryParameter : ""), {
            method: method,
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: publicApi ? "omit" : "include",
            body: fetchBody ? JSON.stringify(fetchBody) : undefined,
        });

        
        if (!response.ok) {
            throw new Error("Jotain meni pieleen.");
        }

        const contentType = response.headers.get("content-type") || "";

        if (contentType.includes("application/json")) {
            return response.json();
        } else {
            return response.text();
        }

    } catch (error) {

        console.error('Api fetch error:', error);
        throw error; // Rethrow the error to handle it in the caller function

    }
}

export enum ApiEndpoint {
    Product = "/api/v1/product",
    Event = "/api/v1/event",
    Auth = "/api/v1/auth",
    Discount = "api/v1/discount/validate",
    Checkout = "/api/v1/checkout",
    CheckAuth = "/api/v1/auth/manage/info",
    Login = "/api/v1/auth/public/login",
    Giftcard = "/api/v1/giftcard"
}

export enum Method {
    GET = "GET",
    POST = "POST",
    PUT = "PUT",
    DELETE = "DELETE",
}
