import ProductModel from "./ProductModel";


export default async function deleteProduct(product: ProductModel): Promise<Response> {
    try {

        // Determine the current URL and append the API endpoint
        //const endpoint = `${window.location.origin}/api/v1/product`;
        const endpoint = `http://localhost:5132/api/v1/product/` + product.code;

        // Make the POST request
        const response = await fetch(endpoint, {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
            },
        });

        // Check for HTTP errors
        if (!response.ok) {
            throw new Error(`Error! Status: ${response.status}`);
        }

        return response; // Return the fetch Response object
    } catch (error) {
        console.error("Error updating product:", error);
        throw error;
    }
}
