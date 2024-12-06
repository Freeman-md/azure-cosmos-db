function deleteAllProducts() {
    const context = getContext();
    const container = context.getCollection();
    const response = context.getResponse();

    const filterQuery = 'SELECT * FROM c'; // Query to get all documents in the partition
    const isQueryAccepted = container.queryDocuments(container.getSelfLink(), filterQuery, {}, function (err, documents) {
        if (err) throw new Error("Error during query: " + err.message);

        if (!documents || documents.length === 0) {
            response.setBody("No products found to delete.");
            return;
        }

        let deleteCount = 0;
        for (let i = 0; i < documents.length; i++) {
            container.deleteDocument(documents[i]._self, {}, function (deleteErr) {
                if (deleteErr) throw new Error("Error deleting document: " + deleteErr.message);

                deleteCount++;
                if (deleteCount === documents.length) {
                    response.setBody(`${deleteCount} products deleted successfully.`);
                }
            });
        }
    });

    if (!isQueryAccepted) {
        throw new Error("Query not accepted.");
    }
}
