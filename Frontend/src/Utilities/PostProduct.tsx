import ProductModel from "./ProductModel";


export default async function postProduct(product: ProductModel): Promise<Response> {
    try {
        // Serialize the object to JSON
        const serializedData = JSON.stringify(product);

        // Determine the current URL and append the API endpoint
        //const endpoint = `${window.location.origin}/api/v1/product`;
        const endpoint = `http://localhost:5132/api/v1/product`;

        // Make the POST request
        const response = await fetch(endpoint, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: serializedData,
            credentials:'include'
        });

        // Check for HTTP errors
        if (!response.ok) {
            throw new Error(`Error! Status: ${response.status}`);
        }

        return response; // Return the fetch Response object
    } catch (error) {
        console.error("Error posting product:", error);
        throw error;
    }
}
