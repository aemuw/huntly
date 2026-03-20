const API_URL = 'http://localhost:5000';

function getToken() {
    return localStorage.getItem('token');
}

function getUser() {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
}

function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = 'login.html';
}

function requireAuth() {
    if (!getToken()) {
        window.location.href = 'login.html';
        return false;
    }
    return true;
}

async function apiRequest(endpoint, method = 'GET', body = null) {
    const options = {
        method,
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${getToken()}`
        }
    };

    if (body) options.body = JSON.stringify(body);

    const response = await fetch(`${API_URL}${endpoint}`, options);

    if (response.status === 401 || response.status === 403) {
        logout();
        return null;
    }

    return response;
}

const STATUS_LABELS = {
    'Watchlist': 'Розгляд',
    'Preparing': 'Готуюсь',
    'Applied': 'Подав',
    'PhoneScreen': 'Дзвінок HR',
    'Technical': 'Технічне',
    'Final': 'Фінал',
    'Offer': 'Офер',
    'Accepted': 'Прийнятий',
    'Rejected': 'Відмова',
    'Ghosted': 'Ігнорують',
    'Withdrawn': 'Відмовився'
};

const PRIORITY_LABELS = {
    'Low': 'Низький',
    'Medium': 'Середній',
    'High': 'Високий'
};

function getBadgeClass(status) {
    const map = {
        'Watchlist': 'badge-watchlist',
        'Preparing': 'badge-preparing',
        'Applied': 'badge-applied',
        'PhoneScreen': 'badge-phonescreen',
        'Technical': 'badge-technical',
        'Final': 'badge-final',
        'Offer': 'badge-offer',
        'Accepted': 'badge-accepted',
        'Rejected': 'badge-rejected',
        'Ghosted': 'badge-ghosted',
        'Withdrawn': 'badge-withdrawn'
    };
    return map[status] || 'badge-watchlist';
}

function formatDate(dateStr) {
    if (!dateStr)
        return '—';
    return new Date(dateStr).toLocaleDateString('uk-UA');
}

function formatSalary(from, to) {
    if (!from && !to)
        return '—';
    if (from && to)
        return `$${from}–$${to}`;
    if (from)
        return `від $${from}`;
    return `до $${to}`;
}

let allApplications = [];

async function loadApplications() {
    if (!requireAuth())
        return;

    const user = getUser();
    if (user) {
        document.getElementById('user-name').textContent = user.firstName;
    }

    const response = await apiRequest('/api/jobapplications');
    if (!response)
        return;

    allApplications = await response.json();
    renderStats(allApplications);
    renderTable(allApplications);
    await loadCompaniesForSelect();
}

function renderStats(apps) {
    const active = ['Applied', 'PhoneScreen', 'Technical', 'Final'];
    const interviews = ['PhoneScreen', 'Technical', 'Final'];
    const offers = ['Offer', 'Accepted'];

    document.getElementById('stat-total').textContent = apps.length;
    document.getElementById('stat-active').textContent =
        apps.filter(a => active.includes(a.status)).length;
    document.getElementById('stat-interviews').textContent =
        apps.filter(a => interviews.includes(a.status)).length;
    document.getElementById('stat-offers').textContent =
        apps.filter(a => offers.includes(a.status)).length;
}

function renderTable(apps) {
    const tbody = document.getElementById('applications-table');

    if (apps.length === 0) {
        tbody.innerHTML = `
            <tr><td colspan="7">
                <div class="empty-state">
                    <p>Заявок ще немає. Додай першу!</p>
                </div>
            </td></tr>`;
        return;
    }

    tbody.innerHTML = apps.map(app => `
        <tr>
            <td>
                <strong>${app.title}</strong>
                ${app.jobUrl
            ? `<br><a href="${app.jobUrl}" target="_blank" 
                        style="color:var(--accent);font-size:0.8rem">
                        Посилання ↗</a>`
            : ''}
            </td>
            <td>${app.companyName || '—'}</td>
            <td>
                <select class="badge ${getBadgeClass(app.status)}"
                    onchange="changeStatus('${app.id}', this.value)"
                    style="border:none;cursor:pointer;background:transparent;
                           font-weight:600;font-size:0.75rem">
                    ${Object.entries(STATUS_LABELS).map(([val, label]) =>
                `<option value="${val}" ${app.status === val ? 'selected' : ''}>
                            ${label}
                        </option>`
            ).join('')}
                </select>
            </td>
            <td>${PRIORITY_LABELS[app.priority] || app.priority}</td>
            <td>${formatSalary(app.salaryFrom, app.salaryTo)}</td>
            <td>${formatDate(app.appliedDate)}</td>
            <td>
                <button class="btn-danger" 
                    onclick="deleteApplication('${app.id}')">
                    Видалити
                </button>
            </td>
        </tr>
    `).join('');
}

function filterApplications() {
    const status = document.getElementById('filter-status').value;
    const filtered = status
        ? allApplications.filter(a => a.status === status)
        : allApplications;
    renderTable(filtered);
}

async function changeStatus(id, newStatus) {
    await apiRequest(`/api/jobapplications/${id}/status`, 'PUT', {
        status: newStatus
    });
    await loadApplications();
}

async function deleteApplication(id) {
    if (!confirm('Видалити цю заявку?'))
        return;
    await apiRequest(`/api/jobapplications/${id}`, 'DELETE');
    await loadApplications();
}

function openModal() {
    document.getElementById('modal-overlay').classList.add('active');
}

function closeModal() {
    document.getElementById('modal-overlay').classList.remove('active');
    document.getElementById('modal-error').classList.add('hidden');
}

async function loadCompaniesForSelect() {
    const response = await apiRequest('/api/companies');
    if (!response)
        return;

    const companies = await response.json();
    const select = document.getElementById('job-company');

    if (!select)
        return;

    select.innerHTML = companies.map(c =>
        `<option value="${c.id}">${c.name}</option>`
    ).join('');

    if (companies.length === 0) {
        select.innerHTML = '<option value="">Спочатку додай компанію</option>';
    }
}

async function createApplication() {
    const title = document.getElementById('job-title').value.trim();
    const companyId = document.getElementById('job-company').value;
    const priority = parseInt(document.getElementById('job-priority').value);
    const salaryFrom = document.getElementById('job-salary-from').value;
    const salaryTo = document.getElementById('job-salary-to').value;
    const jobUrl = document.getElementById('job-url').value.trim();
    const notes = document.getElementById('job-notes').value.trim();
    const errorEl = document.getElementById('modal-error');

    if (!title || !companyId) {
        errorEl.textContent = 'Заповніть назву позиції та оберіть компанію';
        errorEl.classList.remove('hidden');
        return;
    }

    const response = await apiRequest('/api/jobapplications', 'POST', {
        title,
        companyId,
        priority,
        salaryFrom: salaryFrom ? parseFloat(salaryFrom) : null,
        salaryTo: salaryTo ? parseFloat(salaryTo) : null,
        jobUrl: jobUrl || null,
        notes: notes || null
    });

    if (!response || !response.ok) {
        errorEl.textContent = 'Помилка при створенні заявки';
        errorEl.classList.remove('hidden');
        return;
    }

    closeModal();
    await loadApplications();
}

const COMPANY_TYPE_LABELS = {
    'Product':   'Продукт',
    'Outsource': 'Аутсорс',
    'Outstuff':  'Аутстаф',
    'Startup':   'Стартап',
    'Mixed':     'Змішаний',
    'Unknown':   'Невідомо'
};

const COMPANY_SIZE_LABELS = {
    'Startup': 'до 50',
    'Small':   '50–200',
    'Medium':  '200–1000',
    'Large':   '1000+',
    'Unknown': 'Невідомо'
};

async function loadCompanies() {
    if (!requireAuth()) 
        return;

    const user = getUser();
    if (user) {
        const el = document.getElementById('user-name');
        if (el) el.textContent = user.firstName;
    }

    const response = await apiRequest('/api/companies');
    if (!response) 
        return;

    const companies = await response.json();
    renderCompanies(companies);
}

function renderCompanies(companies) {
    const tbody = document.getElementById('companies-table');
    if (!tbody)
        return;

    if (companies.length === 0) {
        tbody.innerHTML = `
            <tr><td colspan="6">
                <div class="empty-state">
                    <p>Компаній ще немає. Додай першу!</p>
                </div>
            </td></tr>`;
        return;
    }

    tbody.innerHTML = companies.map(c => `
        <tr>
            <td><strong>${c.name}</strong></td>
            <td>${COMPANY_TYPE_LABELS[c.type] || c.type}</td>
            <td>${COMPANY_SIZE_LABELS[c.size] || c.size}</td>
            <td>${c.website
                ? `<a href="${c.website}" target="_blank" style="color:var(--accent)">Сайт ↗</a>`
                : '—'}</td>
            <td>${c.linkedIn
                ? `<a href="${c.linkedIn}" target="_blank" style="color:var(--accent)">LinkedIn ↗</a>`
                : '—'}</td>
            <td>
                <button class="btn-danger" onclick="deleteCompany('${c.id}')">
                    Видалити
                </button>
            </td>
        </tr>
    `).join('');
}

async function createCompany() {
    const name     = document.getElementById('company-name').value.trim();
    const type     = parseInt(document.getElementById('company-type').value);
    const size     = parseInt(document.getElementById('company-size').value);
    const website  = document.getElementById('company-website').value.trim();
    const linkedIn = document.getElementById('company-linkedin').value.trim();
    const notes    = document.getElementById('company-notes').value.trim();
    const errorEl  = document.getElementById('modal-error');

    if (!name) {
        errorEl.textContent = 'Введіть назву компанії';
        errorEl.classList.remove('hidden');
        return;
    }

    const response = await apiRequest('/api/companies', 'POST', {
        name, type, size,
        website:  website  || null,
        linkedIn: linkedIn || null,
        notes:    notes    || null
    });

    if (!response || !response.ok) {
        const data = await response.json();
        errorEl.textContent = data.message || 'Помилка при створенні компанії';
        errorEl.classList.remove('hidden');
        return;
    }

    closeModal();
    await loadCompanies();
}

async function deleteCompany(id) {
    if (!confirm('Видалити цю компанію?')) 
        return;
    await apiRequest(`/api/companies/${id}`, 'DELETE');
    await loadCompanies();
}

if (document.getElementById('applications-table')) {
    loadApplications();
}

if (document.getElementById('companies-table')) {
    loadCompanies();
}