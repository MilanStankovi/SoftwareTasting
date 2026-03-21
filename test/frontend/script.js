const entityConfigs = {
  organizers: {
    label: "Organizers",
    endpoint: "organizers",
    fields: [
      { key: "name", label: "Name", type: "text", required: true },
      { key: "email", label: "Email", type: "email", required: true },
      { key: "phone", label: "Phone", type: "text", required: true },
      { key: "companyName", label: "Company Name", type: "text", required: true },
      { key: "taxId", label: "Tax ID", type: "text", required: true },
      { key: "address", label: "Address", type: "text", required: true },
      { key: "city", label: "City", type: "text", required: true },
      { key: "country", label: "Country", type: "text", required: true },
      { key: "isVerified", label: "Is Verified", type: "checkbox" }
    ]
  },
  venues: {
    label: "Venues",
    endpoint: "venues",
    fields: [
      { key: "name", label: "Name", type: "text", required: true },
      { key: "address", label: "Address", type: "text", required: true },
      { key: "city", label: "City", type: "text", required: true },
      { key: "country", label: "Country", type: "text", required: true },
      { key: "capacity", label: "Capacity", type: "number", required: true },
      { key: "hasParking", label: "Has Parking", type: "checkbox" },
      { key: "indoorOutdoorType", label: "Indoor/Outdoor", type: "text", required: true },
      { key: "contactEmail", label: "Contact Email", type: "email", required: true },
      { key: "contactPhone", label: "Contact Phone", type: "text", required: true },
      { key: "rating", label: "Rating", type: "number", step: "0.1", required: true }
    ]
  },
  events: {
    label: "Events",
    endpoint: "events",
    fields: [
      { key: "title", label: "Title", type: "text", required: true },
      { key: "description", label: "Description", type: "textarea", required: true },
      { key: "category", label: "Category", type: "text", required: true },
      { key: "startDate", label: "Start Date", type: "datetime-local", required: true },
      { key: "endDate", label: "End Date", type: "datetime-local", required: true },
      { key: "status", label: "Status", type: "text", required: true },
      { key: "maxAttendees", label: "Max Attendees", type: "number", required: true },
      { key: "basePrice", label: "Base Price", type: "number", step: "0.01", required: true },
      { key: "currency", label: "Currency", type: "text", required: true },
      { key: "organizerId", label: "Organizer ID", type: "text", required: true },
      { key: "venueId", label: "Venue ID", type: "text", required: true },
      { key: "tags", label: "Tags (comma separated)", type: "text" }
    ]
  },
  registrations: {
    label: "Registrations",
    endpoint: "registrations",
    fields: [
      { key: "eventId", label: "Event ID", type: "text", required: true },
      { key: "attendeeFullName", label: "Attendee Full Name", type: "text", required: true },
      { key: "attendeeEmail", label: "Attendee Email", type: "email", required: true },
      { key: "ticketType", label: "Ticket Type", type: "text", required: true },
      { key: "pricePaid", label: "Price Paid", type: "number", step: "0.01", required: true },
      { key: "paymentStatus", label: "Payment Status", type: "text", required: true },
      { key: "checkInStatus", label: "Check-In Status", type: "text", required: true },
      { key: "registeredAt", label: "Registered At", type: "datetime-local", required: true },
      { key: "cancellationReason", label: "Cancellation Reason", type: "text" },
      { key: "notes", label: "Notes", type: "textarea" }
    ]
  }
};

const tabsEl = document.getElementById("tabs");
const entityTitleEl = document.getElementById("entityTitle");
const createFieldsEl = document.getElementById("createFields");
const updateFieldsEl = document.getElementById("updateFields");
const createFormEl = document.getElementById("createForm");
const updateFormEl = document.getElementById("updateForm");
const getByIdFormEl = document.getElementById("getByIdForm");
const deleteFormEl = document.getElementById("deleteForm");
const loadAllBtnEl = document.getElementById("loadAllBtn");
const searchInputEl = document.getElementById("searchInput");
const responseBoxEl = document.getElementById("responseBox");
const tableHeadEl = document.getElementById("resultsHead");
const tableBodyEl = document.getElementById("resultsBody");
const apiBaseUrlEl = document.getElementById("apiBaseUrl");
const seedBtnEl = document.getElementById("seedBtn");
const statsEl = document.getElementById("stats");
const entitySubtitleEl = document.getElementById("entitySubtitle");
const apiStatusTextEl = document.getElementById("apiStatusText");

let currentEntityKey = "organizers";
let loadedRows = [];

function getEntityCountsFromRows(entity, rows) {
  if (!Array.isArray(rows)) return 0;
  return rows.length;
}

function renderStats() {
  const keys = Object.keys(entityConfigs);
  statsEl.innerHTML = keys
    .map((key) => {
      const activeMark = key === currentEntityKey ? " (active)" : "";
      const value = key === currentEntityKey ? getEntityCountsFromRows(key, loadedRows) : "-";
      return `
        <article class="stat-card">
          <div class="stat-title">${entityConfigs[key].label}${activeMark}</div>
          <div class="stat-value">${value}</div>
        </article>
      `;
    })
    .join("");
}

function getApiBaseUrl() {
  return apiBaseUrlEl.value.trim().replace(/\/+$/, "");
}

function toInputDateTimeValue(value) {
  if (!value) return "";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "";
  return new Date(date.getTime() - date.getTimezoneOffset() * 60000)
    .toISOString()
    .slice(0, 16);
}

function fromInputDateTimeValue(value) {
  if (!value) return null;
  return new Date(value).toISOString();
}

function createFieldElement(field, mode) {
  const wrapper = document.createElement("div");
  wrapper.className = "fields";

  const label = document.createElement("label");
  label.textContent = field.label;
  label.htmlFor = `${mode}_${field.key}`;
  wrapper.appendChild(label);

  let input;
  if (field.type === "textarea") {
    input = document.createElement("textarea");
  } else {
    input = document.createElement("input");
    input.type = field.type;
  }

  input.id = `${mode}_${field.key}`;
  input.name = field.key;
  if (field.required) input.required = true;
  if (field.step) input.step = field.step;

  wrapper.appendChild(input);
  return wrapper;
}

function buildFormsForEntity(config) {
  createFieldsEl.innerHTML = "";
  updateFieldsEl.innerHTML = "";

  config.fields.forEach((field) => {
    createFieldsEl.appendChild(createFieldElement(field, "create"));
    updateFieldsEl.appendChild(createFieldElement(field, "update"));
  });
}

function buildTabs() {
  tabsEl.innerHTML = "";
  Object.entries(entityConfigs).forEach(([key, config]) => {
    const button = document.createElement("button");
    button.className = `tab-btn ${key === currentEntityKey ? "active" : ""}`;
    button.textContent = config.label;
    button.addEventListener("click", () => {
      currentEntityKey = key;
      renderCurrentEntity();
    });
    tabsEl.appendChild(button);
  });
}

function renderCurrentEntity() {
  const config = entityConfigs[currentEntityKey];
  entityTitleEl.textContent = `${config.label} - CRUD`;
  entitySubtitleEl.textContent = `Use forms below to create, update, search and delete ${config.label.toLowerCase()}.`;
  buildTabs();
  buildFormsForEntity(config);
  clearTable();
  loadedRows = [];
  searchInputEl.value = "";
  renderStats();
  responseBoxEl.textContent = "No request yet.";
}

function parseFieldValue(field, rawValue) {
  if (field.type === "checkbox") return Boolean(rawValue);
  if (field.type === "number") return rawValue === "" ? null : Number(rawValue);
  if (field.type === "datetime-local") return fromInputDateTimeValue(rawValue);
  if (field.key === "tags") {
    return rawValue
      .split(",")
      .map((x) => x.trim())
      .filter(Boolean);
  }
  return rawValue;
}

function buildPayload(mode) {
  const config = entityConfigs[currentEntityKey];
  const payload = {};

  config.fields.forEach((field) => {
    const element = document.getElementById(`${mode}_${field.key}`);
    const rawValue = field.type === "checkbox" ? element.checked : element.value.trim();
    const parsedValue = parseFieldValue(field, rawValue);

    if (parsedValue === null && !field.required) return;
    if (parsedValue === "" && !field.required) return;
    payload[field.key] = parsedValue;
  });

  return payload;
}

function fillUpdateFormFromEntity(entity) {
  const config = entityConfigs[currentEntityKey];
  document.getElementById("updateId").value = entity.id || "";

  config.fields.forEach((field) => {
    const element = document.getElementById(`update_${field.key}`);
    const value = entity[field.key];

    if (!element) return;
    if (field.type === "checkbox") {
      element.checked = Boolean(value);
    } else if (field.type === "datetime-local") {
      element.value = toInputDateTimeValue(value);
    } else if (field.key === "tags" && Array.isArray(value)) {
      element.value = value.join(", ");
    } else {
      element.value = value ?? "";
    }
  });
}

async function request(path, options = {}) {
  const url = `${getApiBaseUrl()}/${path}`;
  const response = await fetch(url, {
    headers: { "Content-Type": "application/json" },
    ...options
  });

  let data = null;
  const text = await response.text();
  if (text) {
    try {
      data = JSON.parse(text);
    } catch {
      data = text;
    }
  }

  showResponse({
    status: response.status,
    ok: response.ok,
    url,
    data
  });

  if (!response.ok) {
    apiStatusTextEl.textContent = "API error";
    throw new Error(`Request failed (${response.status})`);
  }

  apiStatusTextEl.textContent = "API connected";
  return data;
}

function showResponse(payload) {
  responseBoxEl.textContent = JSON.stringify(payload, null, 2);
}

function clearTable() {
  tableHeadEl.innerHTML = "";
  tableBodyEl.innerHTML = "";
}

function renderTable(data) {
  if (!Array.isArray(data) || data.length === 0) {
    clearTable();
    loadedRows = [];
    renderStats();
    return;
  }

  const keys = Object.keys(data[0]);
  tableHeadEl.innerHTML = `<tr>${keys.map((k) => `<th>${k}</th>`).join("")}</tr>`;

  tableBodyEl.innerHTML = data
    .map((row) => {
      const cells = keys
        .map((key) => {
          const value = Array.isArray(row[key]) ? row[key].join(", ") : row[key];
          return `<td>${value ?? ""}</td>`;
        })
        .join("");
      return `<tr>${cells}</tr>`;
    })
    .join("");

  loadedRows = data;
  renderStats();
}

function applySearch() {
  const query = searchInputEl.value.trim().toLowerCase();
  if (!query) {
    renderTable(loadedRows);
    return;
  }

  const filtered = loadedRows.filter((row) => JSON.stringify(row).toLowerCase().includes(query));
  const keys = filtered[0] ? Object.keys(filtered[0]) : [];

  if (filtered.length === 0) {
    tableHeadEl.innerHTML = keys.length ? `<tr>${keys.map((k) => `<th>${k}</th>`).join("")}</tr>` : "";
    tableBodyEl.innerHTML = `<tr><td colspan="${Math.max(keys.length, 1)}">No matching records.</td></tr>`;
    return;
  }

  tableHeadEl.innerHTML = `<tr>${keys.map((k) => `<th>${k}</th>`).join("")}</tr>`;
  tableBodyEl.innerHTML = filtered
    .map((row) => {
      const cells = keys
        .map((key) => {
          const value = Array.isArray(row[key]) ? row[key].join(", ") : row[key];
          return `<td>${value ?? ""}</td>`;
        })
        .join("");
      return `<tr>${cells}</tr>`;
    })
    .join("");
}

createFormEl.addEventListener("submit", async (e) => {
  e.preventDefault();
  try {
    const config = entityConfigs[currentEntityKey];
    const payload = buildPayload("create");
    const created = await request(config.endpoint, {
      method: "POST",
      body: JSON.stringify(payload)
    });
    createFormEl.reset();
    if (created && created.id) {
      document.getElementById("getByIdInput").value = created.id;
      fillUpdateFormFromEntity(created);
    }
    loadAllBtnEl.click();
  } catch (error) {
    showResponse({ error: error.message });
  }
});

updateFormEl.addEventListener("submit", async (e) => {
  e.preventDefault();
  try {
    const config = entityConfigs[currentEntityKey];
    const id = document.getElementById("updateId").value.trim();
    const payload = buildPayload("update");
    await request(`${config.endpoint}/${id}`, {
      method: "PUT",
      body: JSON.stringify(payload)
    });
    loadAllBtnEl.click();
  } catch (error) {
    showResponse({ error: error.message });
  }
});

getByIdFormEl.addEventListener("submit", async (e) => {
  e.preventDefault();
  try {
    const config = entityConfigs[currentEntityKey];
    const id = document.getElementById("getByIdInput").value.trim();
    const item = await request(`${config.endpoint}/${id}`);
    fillUpdateFormFromEntity(item);
    renderTable(item ? [item] : []);
  } catch (error) {
    showResponse({ error: error.message });
  }
});

deleteFormEl.addEventListener("submit", async (e) => {
  e.preventDefault();
  try {
    const config = entityConfigs[currentEntityKey];
    const id = document.getElementById("deleteIdInput").value.trim();
    await request(`${config.endpoint}/${id}`, { method: "DELETE" });
    loadAllBtnEl.click();
  } catch (error) {
    showResponse({ error: error.message });
  }
});

loadAllBtnEl.addEventListener("click", async () => {
  try {
    const config = entityConfigs[currentEntityKey];
    const items = await request(config.endpoint);
    renderTable(items);
  } catch (error) {
    showResponse({ error: error.message });
  }
});

seedBtnEl.addEventListener("click", async () => {
  try {
    const url = `${getApiBaseUrl().replace(/\/api$/, "")}/api/seed`;
    const response = await fetch(url, {
      method: "POST",
      headers: { "Content-Type": "application/json" }
    });
    const data = await response.json();
    showResponse({ status: response.status, ok: response.ok, url, data });
    apiStatusTextEl.textContent = response.ok ? "Seed completed" : "Seed failed";
  } catch (error) {
    showResponse({ error: error.message });
    apiStatusTextEl.textContent = "API error";
  }
});

searchInputEl.addEventListener("input", applySearch);
apiBaseUrlEl.addEventListener("change", () => {
  apiStatusTextEl.textContent = "API URL updated";
});

renderCurrentEntity();
