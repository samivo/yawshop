
export async function ApiV1(endpoint: ApiEndpoint, method: Method, publicApi: boolean, fetchBody?: any, queryParameter?: string): Promise<any> {

    try {

        const response = await fetch(endpoint + (publicApi ? "/public" : "") + (queryParameter? queryParameter : ""), {
            method: method,
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: publicApi ? "omit" : "include",
            body: (method === Method.POST || method === Method.PUT || method === Method.DELETE) ? JSON.stringify(fetchBody) : undefined,
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
        }
        console.log("API Response Headers:", response.headers);
        const contentType = response.headers.get("content-type") || "";

        if (contentType.includes("application/json")) {
            return response.json();
        } else {
            return response.text();
        }

    } catch (error) {
        console.error('Api fetch error:', error);
        console.error('Api fetch error details:', error.message, error.stack);
        throw error; // Rethrow the error to handle it in the caller function
    }
}

export {
    ApiEndpoint,
    Method
}

enum ApiEndpoint {
    Product = "/api/v1/product",
    Event = "/api/v1/event",
    Auth = "/api/v1/auth",
    Discount = "api/v1/discount/validate",
    Checkout = "/api/v1/checkout",
    CheckAuth = "/api/v1/auth/manage/info",
    Login = "/api/v1/auth/public/login",
    Giftcard = "/api/v1/giftcard",
    Orders = "/api/v1/checkout/all"
}

enum Method {
    GET = "GET",
    POST = "POST",
    PUT = "PUT",
    DELETE = "DELETE",
}
