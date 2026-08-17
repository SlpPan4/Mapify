const url = 'http://localhost:5000/'

let submissions_strats = fetch(`${url}api/submissions/admin/strats`).then(response => {
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    return response.json();
  })
  .catch(error => console.error('Error:', error));

let submissions_categories = fetch(`${url}api/submissions/admin/categories`).then(response => {
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    return response.json();
  })
  .catch(error => console.error('Error:', error));

console.log('strats:', submissions_strats)
console.log('categories:', submissions_categories)