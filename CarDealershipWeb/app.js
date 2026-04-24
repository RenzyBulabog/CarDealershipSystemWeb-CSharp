const API = "http://localhost:5219/api/";

/* ================= LOGIN ================= */
async function login() {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

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
    const car = {
        brand: brand.value,
        model: model.value,
        year: parseInt(year.value),
        price: parseFloat(price.value),
        status: status.value,
        stock: parseInt(stock.value)
    };

    await fetch(API + "cars", {
        method: "POST",
        headers: {"Content-Type":"application/json"},
        body: JSON.stringify(car)
    });

    loadCars();
}

async function updateCar() {
    const car = {
        id: parseInt(id.value),
        brand: brand.value,
        model: model.value,
        year: parseInt(year.value),
        price: parseFloat(price.value),
        status: status.value,
        stock: parseInt(stock.value)
    };

    await fetch(API + "cars/" + car.id, {
        method: "PUT",
        headers: {"Content-Type":"application/json"},
        body: JSON.stringify(car)
    });

    loadCars();
}

async function deleteCar(id) {
    if (!confirm("Delete?")) return;

    await fetch(API + "cars/" + id, { method: "DELETE" });
    loadCars();
}

/* ================= CUSTOMER ================= */
async function loadCustomerCars() {
    const res = await fetch(API + "cars");
    const data = await res.json();

    let rows = "";

    data.forEach(c => {
        rows += `
        <tr>
            <td>${c.brand}</td>
            <td>${c.model}</td>
            <td>${c.year}</td>
            <td>${c.price}</td>
            <td>${c.status}</td>
            <td>
                ${
                    c.stock > 0
                    ? `<button onclick="buyCar(${c.id})">Buy</button>`
                    : `<button disabled>Sold Out</button>`
                }
            </td>
        </tr>`;
    });

    customerTable.innerHTML = rows;
}

async function buyCar(id) {
    const username = localStorage.getItem("username");

    const res = await fetch(API + "sales", {
        method: "POST",
        headers: {"Content-Type":"application/json"},
        body: JSON.stringify({
            carId: id,
            customerName: username
        })
    });

    if (res.ok) {
        alert("Purchased!");
        loadCustomerCars();
        loadDashboardStats();
    } else {
        alert("Out of stock");
    }
}

/* ================= DASHBOARD ================= */
async function loadDashboardStats() {
    const res = await fetch(API + "cars");
    const data = await res.json();

    if (totalCars) totalCars.innerText = data.length;
    if (availableCars) availableCars.innerText = data.filter(c => c.status === "Available").length;
    if (soldCars) soldCars.innerText = data.filter(c => c.status === "Sold").length;
}

async function loadAdminStats() {
    const cars = await fetch(API + "cars").then(r => r.json());
    const sales = await fetch(API + "sales").then(r => r.json());

    let revenue = 0;

    sales.forEach(s => {
        const car = cars.find(c => c.id === s.carId);
        if (car) revenue += car.price;
    });

    if (document.getElementById("totalRevenue"))
        document.getElementById("totalRevenue").innerText = "₱" + revenue.toLocaleString();

    if (document.getElementById("totalSales"))
        document.getElementById("totalSales").innerText = sales.length;
}

/* ================= INVENTORY ================= */
async function loadCharts() {
    const cars = await fetch(API + "cars").then(r => r.json());
    const sales = await fetch(API + "sales").then(r => r.json());

    let available = cars.filter(c => c.status === "Available").length;
    let sold = cars.filter(c => c.status === "Sold").length;

    // 🔥 PIE CHART (Car Status)
    new Chart(document.getElementById("statusChart"), {
        type: "pie",
        data: {
            labels: ["Available", "Sold"],
            datasets: [{
                data: [available, sold]
            }]
        }
    });

    // 🔥 BAR CHART (Sales Count)
    new Chart(document.getElementById("salesChart"), {
        type: "bar",
        data: {
            labels: ["Total Sales"],
            datasets: [{
                label: "Cars Sold",
                data: [sales.length]
            }]
        }
    });
}

/* ================= CHARTS ================= */

// 🔥 GLOBAL VARIABLES (IMPORTANT)
let statusChart, salesChart;

async function loadCharts() {

    // 🔥 PREVENT DUPLICATE CHARTS
    if (statusChart) statusChart.destroy();
    if (salesChart) salesChart.destroy();

    const cars = await fetch(API + "cars").then(r => r.json());
    const sales = await fetch(API + "sales").then(r => r.json());

    let available = cars.filter(c => c.status === "Available").length;
    let sold = cars.filter(c => c.status === "Sold").length;

    // 🔥 PIE CHART (Available vs Sold)
    statusChart = new Chart(document.getElementById("statusChart"), {
        type: "pie",
        data: {
            labels: ["Available", "Sold"],
            datasets: [{
                data: [available, sold],
                backgroundColor: [
                    "#22c55e", // green
                    "#ef4444"  // red
                ]
            }]
        },
        options: {
            plugins: {
                legend: {
                    labels: {
                        color: "white"
                    }
                }
            }
        }
    });

    // 🔥 BAR CHART (Total Sales)
    salesChart = new Chart(document.getElementById("salesChart"), {
        type: "bar",
        data: {
            labels: ["Total Sales"],
            datasets: [{
                label: "Cars Sold",
                data: [sales.length],
                backgroundColor: "#38bdf8"
            }]
        },
        options: {
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

    let rows = "";

    data.forEach(c => {
        rows += `
        <tr>
            <td>${c.id}</td>
            <td>${c.brand}</td>
            <td>${c.model}</td>
            <td>${c.stock}</td>
            <td>
                <button onclick="updateStock(${c.id})">Update</button>
            </td>
        </tr>`;
    });

    inventoryTable.innerHTML = rows;
}

async function updateStock(id) {
    const newStock = prompt("New stock:");
    if (!newStock) return;

    const res = await fetch(API + "cars/" + id);
    const car = await res.json();

    car.stock = parseInt(newStock);

    await fetch(API + "cars/" + id, {
        method: "PUT",
        headers: {"Content-Type":"application/json"},
        body: JSON.stringify(car)
    });

    loadInventory();
}

/* ================= SALES ================= */
async function loadSales() {
    const res = await fetch(API + "sales");
    const data = await res.json();

    let rows = "";

    data.forEach(s => {
        rows += `
        <tr>
            <td>${s.id}</td>
            <td>${s.carId}</td>
            <td>${s.customerName}</td>
            <td>${new Date(s.saleDate).toLocaleString()}</td>
        </tr>`;
    });

    salesTable.innerHTML = rows;
}

/* ================= PROTECT ================= */
if (location.href.includes("admin") && localStorage.getItem("role") !== "admin") {
    window.location.href = "index.html";
}

if (location.href.includes("customer") && localStorage.getItem("role") !== "customer") {
    window.location.href = "index.html";
}

/* ================= AUTO LOAD ================= */
window.onload = () => {
    if (typeof loadDashboardStats === "function")
        loadDashboardStats();

    if (typeof loadAdminStats === "function")
        loadAdminStats();

    // 🔥 LOAD CHARTS (IMPORTANT)
    if (document.getElementById("statusChart")) {
        loadCharts();
    }
};