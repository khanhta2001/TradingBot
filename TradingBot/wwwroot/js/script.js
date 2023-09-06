// document.getElementById('searchForm').addEventListener('submit', function(event) {
//     event.preventDefault();
//
//     // Fetch data from SearchProducts
//     var userName = document.getElementById('userName').value;
//     var searchWord = document.getElementById('searchResultsInput').value;
//     fetch('/SearchProducts', {
//         method: 'POST',
//         headers: {
//             'Content-Type': 'application/json'
//         },
//         body: JSON.stringify({
//             userName: userName,
//             searchWord: searchWord
//         })
//     })
//         .then(response => response.json())
//         .then(data => populateTable(data))
//         .catch(error => console.error('Error:', error));
// });
//
// function populateTable(data) {
//     const tableBody = document.getElementById('searchResultsTable').querySelector('tbody');
//     tableBody.innerHTML = ''; // Clear previous results
//
//     if (data && data.length) {
//         data.forEach(item => {
//             const row = tableBody.insertRow();
//             row.insertCell(0).textContent = item.stockSymbol;
//             row.insertCell(1).textContent = item.productName;
//             row.insertCell(2).textContent = item.currentPrice;
//         });
//
//         // Display the table
//         document.getElementById('searchResultsTable').classList.remove('d-none');
//     } else {
//         // Handle the no results case if needed
//     }
// }
