var Sortable = {
    baseUrl: "",
    sortBy: 0,
    searchTerm: "",
    search() {
        var searchKey = $("#txtSearch").val();
        window.location.href = Sortable.baseUrl + searchKey;
    },
    Sort(sortBy) {
        window.location.href = Sortable.baseUrl + "?sortBy=" + sortBy;
    }
};
