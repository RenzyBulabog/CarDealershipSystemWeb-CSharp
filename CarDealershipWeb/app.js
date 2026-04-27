const API = "http://localhost:5219/api/";

/* ================= LOGIN ================= */
async function login() {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    const fullname = document.getElementById("regFullname").value;
    const email = document.getElementById("regEmail").value;

    const res = await fetch(API + "auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password })
    });

    if (!res.ok) {
        alert("Login Failed");
        return;
    }

    const data = await res.json();

    localStorage.setItem("username", data.username);
    localStorage.setItem("role", data.role);

    if (data.role === "admin") {
        window.location.href = "admin-dashboard.html";
    } else {
        window.location.href = "customer-dashboard.html";
    }
}

async function register() {
    console.log("REGISTER CLICKED");

    const username = document.getElementById("regUsername").value;
    const password = document.getElementById("regPassword").value;
    const role = document.getElementById("regRole").value;

    // 🔥 ADD THESE
    const fullname = document.getElementById("regFullname").value;
    const email = document.getElementById("regEmail").value;

    try {
        const res = await fetch(API + "auth/register", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                username: username,
                password: password,
                role: role,
                fullname: fullname,
                email: email
            })
        });

        console.log("STATUS:", res.status);

        if (res.ok) {
            alert("Registered Successfully!");
            toggleRegister(); // 🔥 auto hide form (optional)
        } else {
            const text = await res.text();
            alert("Error: " + text);
        }

    } catch (err) {
        console.error(err);
        alert("API ERROR");
    }
}

/* ================= ADMIN CARS ================= */
async function loadCars() {
    const res = await fetch(API + "cars");
    const data = await res.json();

    let rows = "";
    data.forEach(c => {
        rows += `
        <tr>
            <td>${c.id}</td>
            <td>${c.brand}</td>
            <td>${c.model}</td>
            <td>${c.year}</td>
            <td>${c.price}</td>
            <td>${c.status}</td>
            <td>${c.stock}</td>
            <td>
                <button onclick="fill(${c.id}, '${c.brand}', '${c.model}', ${c.year}, ${c.price}, '${c.status}', ${c.stock})">Edit</button>
                <button onclick="deleteCar(${c.id})">Delete</button>
            </td>
        </tr>`;
    });

    carsTable.innerHTML = rows;
}

function fill(i, b, m, y, p, s, st) {
    id.value = i;
    brand.value = b;
    model.value = m;
    year.value = y;
    price.value = p;
    status.value = s;
    stock.value = st;
}

async function addCar() {
    console.log("ADD CLICKED");

    const car = {
        brand: document.getElementById("brand").value,
        model: document.getElementById("model").value,
        year: parseInt(document.getElementById("year").value),
        price: parseFloat(document.getElementById("price").value),
        status: document.getElementById("status").value,
        stock: 0 // 🔥 FIX (since wala input)
    };

    console.log(car);

    const res = await fetch(API + "cars", {
        method: "POST",
        headers: {"Content-Type":"application/json"},
        body: JSON.stringify(car)
    });

    console.log("STATUS:", res.status);

    if (res.ok) {
        alert("Car Added");
        loadCars();
    } else {
        alert("Add Failed");
    }
}

async function updateCar() {
    console.log("UPDATE CLICKED");

    const idVal = document.getElementById("id").value;

    const car = {
        id: parseInt(idVal),
        brand: document.getElementById("brand").value,
        model: document.getElementById("model").value,
        year: parseInt(document.getElementById("year").value),
        price: parseFloat(document.getElementById("price").value),
        status: document.getElementById("status").value,
        stock: 0 // 🔥 same fix
    };

    const res = await fetch(API + "cars/" + idVal, {
        method: "PUT",
        headers: {"Content-Type":"application/json"},
        body: JSON.stringify(car)
    });

    console.log("STATUS:", res.status);

    if (res.ok) {
        alert("Updated");
        loadCars();
    } else {
        alert("Update Failed");
    }
}

async function deleteCar(id) {
    if (!confirm("Delete?")) return;

    await fetch(API + "cars/" + id, { method: "DELETE" });
    loadCars();
}

/* ================= CUSTOMER ================= */

// 🔥 LOAD CARS (fixed + disable if no stock)
async function loadCustomerCars() {
    const res = await fetch(API + "cars");
    const data = await res.json();

    let rows = "";

    data.forEach(c => {
        const isAvailable = c.stock > 0;

        rows += `
        <tr>
            <td>${c.brand}</td>
            <td>${c.model}</td>
            <td>${c.year}</td>
            <td>₱${c.price}</td>
            <td>${isAvailable ? "Available" : "Sold Out"}</td>
            <td>
                ${
                    isAvailable
                    ? `<button onclick="buyCar(${c.id})">Buy</button>`
                    : `<button disabled>Sold Out</button>`
                }
            </td>
        </tr>`;
    });

    document.getElementById("customerTable").innerHTML = rows;
}

// 🔥 BUY → creates Pending sale
async function buyCar(id) {
    const username = localStorage.getItem("username");

    if (!username) {
        alert("Please login first");
        return;
    }

    const res = await fetch(API + "sales", {
        method: "POST",
        headers: {"Content-Type":"application/json"},
        body: JSON.stringify({
            carId: id,
            customerName: username,
            Status: "Pending"
        })
    });

    if (res.ok) {
        alert("Request sent! Waiting for approval.");
        loadCustomerCars();
    } else {
        const err = await res.text();
        alert("Error: " + err);
    }
}

/* ================= DASHBOARD ================= */
async function loadDashboardStats() {
    const cars = await fetch(API + "cars").then(r => r.json());
    const sales = await fetch(API + "sales").then(r => r.json());

    // 🔥 APPROVED ONLY
    const approved = sales.filter(s => s.status === "Approved");

    // 🚗 TOTAL CARS (unique cars)
    const totalCarsCount = cars.length;

    // 📦 AVAILABLE = TOTAL STOCK
    const totalStock = cars.reduce((sum, c) => sum + (c.stock || 0), 0);

    // 💰 SOLD = approved only
    const soldCount = approved.length;

    // 🧾 TOTAL SALES = approved only
    const totalSalesCount = approved.length;

    // 💸 REVENUE = approved only
    let revenue = 0;
    approved.forEach(s => {
        const car = cars.find(c => c.id === s.carId);
        if (car) revenue += car.price;
    });

    // 🔥 APPLY VALUES
    if (document.getElementById("totalCars"))
        document.getElementById("totalCars").innerText = totalCarsCount;

    if (document.getElementById("availableCars"))
        document.getElementById("availableCars").innerText = totalStock;

    if (document.getElementById("soldCars"))
        document.getElementById("soldCars").innerText = soldCount;

    if (document.getElementById("totalSales"))
        document.getElementById("totalSales").innerText = totalSalesCount;

    if (document.getElementById("totalRevenue"))
        document.getElementById("totalRevenue").innerText =
            "₱" + revenue.toLocaleString();
}

/* ================= CHARTS ================= */

// 🔥 GLOBAL VARIABLES
let statusChart, salesChart;

async function loadCharts() {

    // 🔥 PREVENT DUPLICATE CHARTS
    if (statusChart) statusChart.destroy();
    if (salesChart) salesChart.destroy();

    const cars = await fetch(API + "cars").then(r => r.json());
    const sales = await fetch(API + "sales").then(r => r.json());

    // 🔥 ONLY APPROVED SALES
    const approved = sales.filter(s => s.status === "Approved");

    // 📦 TOTAL STOCK (AVAILABLE)
    const totalStock = cars.reduce((sum, c) => sum + (c.stock || 0), 0);

    // 💰 SOLD = approved only
    const sold = approved.length;

    // 🔥 PIE CHART (Stock vs Sold)
    statusChart = new Chart(document.getElementById("statusChart"), {
        type: "pie",
        data: {
            labels: ["Available Stock", "Sold"],
            datasets: [{
                data: [totalStock, sold],
                backgroundColor: [
                    "#22c55e", // green
                    "#ef4444"  // red
                ]
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    labels: {
                        color: "white"
                    }
                }
            }
        }
    });

    // 🔥 BAR CHART (APPROVED SALES ONLY)
    salesChart = new Chart(document.getElementById("salesChart"), {
        type: "bar",
        data: {
            labels: ["Approved Sales"],
            datasets: [{
                label: "Cars Sold",
                data: [approved.length],
                backgroundColor: "#38bdf8"
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    ticks: { color: "white" }
                },
                x: {
                    ticks: { color: "white" }
                }
            },
            plugins: {
                legend: {
                    labels: {
                        color: "white"
                    }
                }
            }
        }
    });
}

/* ================= INVENTORY ================= */
async function loadInventory() {
    const res = await fetch(API + "cars");
    const data = await res.json();

    let grouped = {};

    // 🔥 GROUP BY BRAND
    data.forEach(c => {
        if (!grouped[c.brand]) {
            grouped[c.brand] = [];
        }
        grouped[c.brand].push(c);
    });

    let html = "";

    // 🔥 DISPLAY PER BRAND
    for (let brand in grouped) {

        html += `
        <tr>
            <td colspan="5" style="font-weight:bold; background:#1e293b; color:#38bdf8;">
                ${brand}
            </td>
        </tr>
        `;

        grouped[brand].forEach(c => {
            html += `
            <tr>
                <td>${c.id}</td>
                <td>${c.brand}</td>
                <td>${c.model}</td>

                <!-- 🔥 EDITABLE STOCK -->
                <td>
                    <input type="number" id="stock-${c.id}" value="${c.stock ?? 0}" style="width:60px;">
                </td>

                <!-- 🔥 UPDATE BUTTON -->
                <td>
                    <button onclick="updateStock(${c.id})">Update</button>
                </td>
            </tr>
            `;
        });
    }

    document.getElementById("inventoryTable").innerHTML = html;
}

async function loadSalesAdmin() {
    const res = await fetch(API + "Sales");
    const data = await res.json();

    console.log("ADMIN DATA:", data);

    let rows = "";

    data.forEach(s => {

        if (s.status && s.status.toLowerCase() === "pending") {

            rows += `
            <tr>
                <td>${s.id}</td>
                <td>${s.carId}</td>
                <td>${s.customerName}</td>
                <td>${s.status}</td>
                <td>
                    <button onclick="approveSale(${s.id})">Approve</button>
                    <button onclick="declineSale(${s.id})">Decline</button>
                </td>
            </tr>`;
        }
    });

    document.getElementById("salesTable").innerHTML = rows;
}

async function approveSale(saleId) {

    const res = await fetch(API + "sales");
    const sales = await res.json();

    const sale = sales.find(s => s.id == saleId);

    if (!sale) {
        alert("Sale not found");
        return;
    }

    sale.status = "Approved";

    const update = await fetch(API + "sales/" + saleId, {
        method: "PUT",
        headers: {"Content-Type":"application/json"},
        body: JSON.stringify(sale)
    });

    if (update.ok) {
        alert("Approved!");
        loadSalesAdmin();
        loadDashboardStats();
        loadCharts();
    } else {
        const err = await update.text();
        alert(err);
    }
}

async function declineSale(saleId) {

    const res = await fetch(API + "sales");
    const sales = await res.json();

    const sale = sales.find(s => (s.id ?? s.Id) == saleId);

    if (!sale) {
        alert("Sale not found");
        return;
    }

    sale.status = "Declined";

    const update = await fetch(API + "sales/" + saleId, {
        method: "PUT",
        headers: {"Content-Type":"application/json"},
        body: JSON.stringify(sale)
    });

    if (update.ok) {
        alert("Declined!");
        loadSalesAdmin();
        loadDashboardStats(); // 🔥 update stats
        loadCharts();
    } else {
        const err = await update.text();
        alert("Error: " + err);
    }
}

async function sellCar() {
    const carId = parseInt(document.getElementById("carId").value);
    const customer = document.getElementById("customer").value;

    if (!carId || !customer) {
        alert("Fill all fields");
        return;
    }

    const res = await fetch(API + "cars/" + carId);
    const car = await res.json();

    if (car.stock <= 0) {
        alert("No stock!");
        return;
    }

    car.stock -= 1;

    await fetch(API + "cars/" + carId, {
        method: "PUT",
        headers: {"Content-Type":"application/json"},
        body: JSON.stringify(car)
    });

    await fetch(API + "sales", {
        method: "POST",
        headers: {"Content-Type":"application/json"},
        body: JSON.stringify({
            carId: carId,
            customerName: customer,
            status: "Approved" // 🔥 DIRECT
        })
    });

    alert("Sale completed!");
}

async function updateStock(id) {
    const newStock = document.getElementById("stock-" + id).value;

    if (newStock === "" || isNaN(newStock)) {
        alert("Invalid stock");
        return;
    }

    try {
        // 🔥 GET FULL CAR (IMPORTANT)
        const res = await fetch(API + "cars/" + id);
        const car = await res.json();

        // 🔥 UPDATE STOCK
        car.stock = parseInt(newStock);

        // 🔥 SEND FULL OBJECT
        const updateRes = await fetch(API + "cars/" + id, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(car)
        });

        if (updateRes.ok) {
            alert("Stock Updated!");
            loadInventory();
        } else {
            const err = await updateRes.text();
            alert("Update Failed: " + err);
        }

    } catch (err) {
        console.error(err);
        alert("API ERROR");
    }
}

/* ================= SALES ================= */
async function loadSales() {
    const res = await fetch(API + "sales");
    const data = await res.json();

    console.log("SALES DATA:", data);

    let rows = "";

    data.forEach(s => {

        // 🔥 ONLY APPROVED SALES
        if (s.status !== "Approved") return;

        // 🔥 SAFE DATE
        const date = (s.saleDate && s.saleDate !== "0001-01-01T00:00:00")
            ? new Date(s.saleDate).toLocaleString()
            : "-";

        rows += `
        <tr>
            <td>${s.id}</td>
            <td>${s.carId}</td>
            <td>${s.customerName}</td>
            <td>${date}</td>
        </tr>`;
    });

    document.getElementById("salesTable").innerHTML = rows;
}

/* ================= PROTECT ================= */
if (location.href.includes("admin") && localStorage.getItem("role") !== "admin") {
    window.location.href = "index.html";
}

if (location.href.includes("customer") && localStorage.getItem("role") !== "customer") {
    window.location.href = "index.html";
}

function toggleRegister() {
    const form = document.getElementById("registerSection");
    form.style.display = form.style.display === "none" ? "block" : "none";
}

/* ================= AUTO LOAD ================= */
window.onload = () => {
    if (typeof loadDashboardStats === "function")
        loadDashboardStats();

    if (typeof loadAdminStats === "function")
        loadAdminStats();

    if (document.getElementById("salesTable")) {

        // 🔥 FIX HERE
        if (location.pathname.includes("admin-sales-history")) {
            loadSales(); // HISTORY
        } else {
            loadSalesAdmin(); // ADMIN APPROVAL
        }
    }

    if (document.getElementById("statusChart")) {
        loadCharts();
    }
};