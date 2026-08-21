function addRow() {
    const tr = `<tr>
        <td><input type="text" class="form-control item-name" required /></td>
        <td><input type="number" class="form-control item-qty" min="1" value="1" required /></td>
        <td><input type="number" class="form-control item-price" min="0" value="0" required /></td>
        <td><button type="button" class="btn btn-danger btn-sm" onclick="removeRow(this)">X</button></td>
    </tr>`;
    document.querySelector('#itemTable tbody').insertAdjacentHTML('beforeend', tr);
}

function removeRow(btn) {
    if (document.querySelectorAll('#itemTable tbody tr').length > 1) {
        btn.closest('tr').remove();
    }
}

async function submitOrder(salesSoId) {
    const rows = document.querySelectorAll('#itemTable tbody tr');
    const items = [];

    rows.forEach(r => {
        items.push({
            itemName: r.querySelector('.item-name').value,
            quantity: parseInt(r.querySelector('.item-qty').value) || 0,
            price: parseFloat(r.querySelector('.item-price').value) || 0
        });
    });

    const payload = {
        soNo: document.getElementById('soNo').value,
        orderDate: document.getElementById('orderDate').value,
        customerId: parseInt(document.getElementById('customerId').value) || 0,
        address: document.getElementById('address').value,
        items: items
    };

    const res = await fetch(`/Order/Edit/${salesSoId}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    });

    const data = await res.json();

    if (res.ok && data.success) {
        alert(data.message || 'Order berhasil diupdate!');
        window.location.href = '/Order/Index';
    } else {
        const errorMsg = data.errors ? data.errors.join('\n') : data.message;
        alert('Gagal: ' + errorMsg);
    }
}
