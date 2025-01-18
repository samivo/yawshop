
export async function ApiV1(endpoint: ApiEndpoint, method: Method, publicApi: boolean, fetchBody?: any): Promise<any> {

    try {

        const response = await fetch(endpoint + (publicApi ? "/public" : ""), {
            method: method,
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: publicApi ? "omit" : "include",
            body: fetchBody ? JSON.stringify(fetchBody) : undefined,
        });

        if (!response.ok) {

            console.log(response);
            throw new Error("Jotain meni pieleen.");
            
        }

        const data = await response.json();
        return data;

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
}

export enum Method {
    GET = "GET",
    POST = "POST",
    PUT = "PUT",
    DELETE = "DELETE",
}
