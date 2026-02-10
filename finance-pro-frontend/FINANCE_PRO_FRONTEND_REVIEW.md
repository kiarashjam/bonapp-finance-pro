# Finance Pro Frontend – Code Review: Bugs & Issues

Review of all `src/**/*.ts` and `src/**/*.tsx` files for TypeScript mismatches, null safety, API usage, React/Redux patterns, Ant Design usage, error handling, forms, routing, and robustness.

---

## 1. main.tsx

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Line 6 | **Missing null check for `#root`** – `document.getElementById('root')!` uses non-null assertion; if the element is missing (e.g. wrong HTML or test setup), `createRoot(null)` can throw at runtime. | Medium | Guard before render: `const root = document.getElementById('root'); if (root) { ReactDOM.createRoot(root).render(...); } else { console.error('Root element #root not found'); }` |

---

## 2. App.tsx

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Line 50 | **Catch-all route sends to `/dashboard`** – Unauthenticated users hitting an unknown path (e.g. `/unknown`) are first sent to `/dashboard`, then `ProtectedRoute` sends them to `/login`. Works but causes an extra redirect. | Low | Optional: use a single redirect for unauthenticated users (e.g. `path="*"` → `<Navigate to={isAuthenticated ? '/dashboard' : '/login'} />`). |
| Line 31 | **useEffect dependency** – `dispatch` from `useDispatch` is stable; listing it is correct. No change required. | — | — |

---

## 3. api/client.ts

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Line 18–24 | **401 handler uses full page redirect** – `window.location.href = '/login'` causes a full reload and loses in-app state. Works but is heavy-handed. | Low | Prefer in-app redirect (e.g. dispatch logout + navigate via React Router) so SPA state is cleared consistently. |
| Line 12 | **Request interceptor and `config.headers`** – In strict TypeScript setups, `config.headers` could be undefined in typing. Axios usually ensures it exists. | Low | If you see type errors: `(config.headers ??= {} as AxiosRequestHeaders).Authorization = \`Bearer ${token}\`;` or use a type-safe header helper. |

---

## 4. api/index.ts

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Line 31 | **Query param not encoded** – `status` is appended as `&status=${status}`. If `status` contains `&`, `=`, or spaces, the URL can be malformed or ambiguous. | Medium | Use `URLSearchParams` or `encodeURIComponent(status)` when building the query string. Same for other optional query params (e.g. entityType, getPayouts status). |
| Line 78 | **`importStatement` response type unspecified** – Return type is not declared. `ReconciliationPage` uses `res.data.imported`; if the API returns a different shape, this will throw at runtime. | Medium | Type the response (e.g. `client.post<{ imported: number }>(...)`) and optionally handle missing `imported` before using it. |
| Line 79 | **`getPayouts` return type** – Typed as `PayoutDto[]`. If the backend returns a paged result `{ items, totalCount, page, pageSize }`, the type is wrong and code that uses `res.data` as an array may break. | Medium | Confirm API contract; if paged, use `PagedResult<PayoutDto>` and use `res.data.items` (and totalCount) where appropriate. |

---

## 5. store/slices/authSlice.ts

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Lines 24, 36, 45 | **`err: any`** – Weak typing on caught errors. | Low | Use `unknown` and narrow: `catch (err: unknown) { const msg = err instanceof AxiosError ? err.response?.data?.message : 'Login failed'; return rejectWithValue(msg || 'Login failed'); }` (and similar for register/me). |
| Lines 66, 69 | **Rejected `action.payload` may be undefined** – `rejectWithValue` is always used in the catch, but TypeScript types for `rejected` can still allow `payload` to be undefined. Assigning to `state.error` can then store `undefined` while `AuthState.error` is `string \| null`. | Low | Use `state.error = (action.payload as string) ?? null;` so state stays `string \| null`. |

---

## 6. hooks/useAuth.ts

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| — | No issues found. | — | — |

---

## 7. utils/format.ts

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Lines 6–7 | **`formatPercent` never shows minus sign** – `${value >= 0 ? '' : ''}` is the same for both branches, so negative percentages are shown without a minus. | Medium | Use `${value >= 0 ? '' : '-'}${Math.abs(value).toFixed(1)}%` (or equivalent) so negative values display correctly. |
| Line 9 | **`formatDate` with invalid input** – `dayjs(date)` on invalid or non-string `date` can yield Invalid Date; `format()` still returns a string but it may be confusing. | Low | Optionally validate or use `dayjs(date).isValid() ? dayjs(date).format(...) : '—'`. Same for `formatDateTime`. |

---

## 8. components/layout/AppLayout.tsx

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Line 44 | **`menuItems` includes `key` that may not match route** – `selectedKey` is derived from pathname; menu keys are `/dashboard`, `/accounts`, etc. For nested routes like `/reports` the first segment is correct. For `/` exactly, `selectedKey` becomes `'/'`, which matches the parent; the index redirects to `/dashboard`, so the selected menu item might not highlight correctly on initial load. | Low | Ensure index route redirects to `/dashboard` (already in place) and that `selectedKey` for `/` is normalized to `'/dashboard'` when pathname is `/` or `/dashboard`. |
| — | **Null checks** – `user?.firstName`, `user?.organizationName` are used; safe. | — | — |

---

## 9. pages/auth/LoginPage.tsx

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| — | No issues found. Error display and `clearError` are used correctly. | — | — |

---

## 10. pages/auth/RegisterPage.tsx

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Line 14 | **`values: any`** – Form submit handler is untyped. | Low | Type as `RegisterRequest` or a form values type matching the fields (organizationName, firstName, lastName, email, password). |
| Lines 30–31 | **Form rules without message** – `firstName` and `lastName` have `rules={[{ required: true }]}` with no `message`, so Ant Design shows a generic message. | Low | Add `message: 'First name is required'` and similar for last name for consistency with other fields. |

---

## 11. pages/dashboard/DashboardPage.tsx

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Line 16 | **API errors swallowed** – `.catch(() => {})` hides failures; user sees "No data available" with no indication of network/API error. | Medium | Replace with `.catch((err) => { message.error(err?.response?.data?.message ?? 'Failed to load dashboard'); })` (and ensure `message` is imported from `antd`). |
| Line 55 | **Crash if `revenueChart` is missing** – `data.revenueChart` is used in `LineChart`; if the API omits it or returns null/undefined, the component can throw. | Medium | Use optional chaining and fallback: `data.revenueChart ?? []`, and ensure the type allows `revenueChart?` if the API can omit it. |

---

## 12. pages/accounts/AccountsPage.tsx

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Line 17 | **No `.catch()` on `accountsApi.getAll()`** – Promise rejection is unhandled; loading is cleared in `finally`, but the user gets no error message and state may be stale. | Medium | Add `.catch((err) => { message.error(err?.response?.data?.message ?? 'Failed to load accounts'); })` before `finally`. |
| Line 29 | **Error message may show `[object Object]`** – `err.response?.data` is often an object (e.g. `{ message: '...' }`). Passing it to `message.error()` can display `[object Object]`. | Medium | Use `message.error(err?.response?.data?.message ?? err?.response?.data ?? 'Failed to create account');` or a small helper that extracts a string from the error. |
| Line 21 | **`load` in useEffect** – No dependency array issue; `load` is called once on mount. | — | — |

---

## 13. pages/journals/JournalsPage.tsx

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Line 21 | **No `.catch()` on `journalsApi.getAll()`** – Same unhandled rejection and no user feedback as in AccountsPage. | Medium | Add `.catch(...)` and show a message (e.g. via `message.error`). |
| Line 24 | **No `.catch()` on `accountsApi.getAll()`** – If this fails, accounts stay empty and the form Select has no options; user gets no explanation. | Medium | Add `.catch((err) => message.error(err?.response?.data?.message ?? 'Failed to load accounts'));`. |
| Line 28 | **Date/object access** – `values.entryDate.format('YYYY-MM-DD')` will throw if `entryDate` is undefined (e.g. validation bypassed). Form has `required: true`, so low risk. | Low | Defensive: `entryDate: values.entryDate?.format?.('YYYY-MM-DD') ?? ''` and/or validate before submit. |
| Line 30 | **`values.lines`** – If `values.lines` is undefined (e.g. form tampering), `.map` throws. `initialValues` sets `lines`. | Low | Use `(values.lines ?? []).map(...)` to be safe. |
| Line 37, 42 | **Error message object** – Same as AccountsPage; `err.response?.data` may be an object. | Medium | Use `err?.response?.data?.message ?? err?.response?.data ?? '...'` (and ensure message is a string). |
| — | **Debit = Credit** – No client-side validation that total debit equals total credit; server may reject. | Low | Optional: add a form validator that checks sum(debit) === sum(credit) before submit. |

---

## 14. pages/expenses/ExpensesPage.tsx

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Lines 25, 28 | **No `.catch()` on `expensesApi.getAll()` and `vendorsApi.getAll()`** – Unhandled rejections and no user feedback. | Medium | Add `.catch(...)` with `message.error` for both calls. |
| Line 34 | **Error message object** – Same as above; may show `[object Object]`. | Medium | Use `err?.response?.data?.message ?? err?.response?.data ?? 'Failed'`. |
| Line 31 | **`values.expenseDate.format`** – Same as journals; required in form but defensive check is safer. | Low | Optional: `values.expenseDate?.format?.('YYYY-MM-DD')`. |

---

## 15. pages/invoices/InvoicesPage.tsx

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Lines 22, 25 | **No `.catch()` on `invoicesApi.getAll()` and `customersApi.getAll()`** – Same pattern as above. | Medium | Add `.catch(...)` and user-visible error messages. |
| Line 37 | **Error message object** – Same as above. | Medium | Extract message string from `err.response?.data`. |
| Line 40 | **`handleStatusChange` error** – `message.error('Failed')` ignores server message. | Low | Use `message.error(err?.response?.data?.message ?? 'Failed');` for consistency. |
| Lines 31–33 | **Date and lines** – `invoiceDate`/`dueDate` and `lines` are required in form; optional defensive checks (e.g. `?.format`, `lines ?? []`) reduce risk. | Low | Optional. |

---

## 16. pages/sales/SalesPage.tsx

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Line 24 | **No `.catch()` on `salesApi.getDailySummaries()`** – Unhandled rejection and no loading-state feedback on error. | Medium | Add `.catch((err) => { message.error(...); setSummaries([]); }).finally(...)`. |
| Lines 33, 40, 47 | **Error message object** – Same pattern; use `err?.response?.data?.message ?? err?.response?.data ?? '...'`. | Medium | Standardize error extraction. |
| Lines 44–46 | **`confirmCloseDay` uses `selectedDate`** – If `selectedDate` is never set or is cleared (e.g. modal closed and reopened in an edge case), `salesApi.closeDay(selectedDate)` could be called with `''`. | Low | Guard: `if (!selectedDate) { message.error('No date selected'); return; }` before calling the API. |
| Line 31 | **Manual entry payload** – Form has initialValues for optional fields; spreading `values` and overriding `date` matches `ManualSalesEntryRequest`. | — | — |

---

## 17. pages/reconciliation/ReconciliationPage.tsx

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Line 13 | **`loading` state never used** – `const [, setLoading] = useState(true)`; UI never shows a loading indicator, so the page can look stuck while data loads. | Medium | Use loading: `const [loading, setLoading] = useState(true)` and pass `loading={loading}` to a `Spin` wrapper or table, or show a skeleton. |
| Line 17–19 | **No `.catch()` on `Promise.all`** – If either request fails, the promise rejects with no user feedback and loading is still cleared. | Medium | Add `.catch((err) => { message.error(err?.response?.data?.message ?? 'Failed to load reconciliation data'); })` before `finally`. |
| Line 26 | **`res.data.imported`** – Type of `importStatement` response is not declared; if the API returns a different shape, this will throw. | Medium | Type the import response (e.g. `{ imported: number }`) and optionally use `res.data?.imported ?? 0` for the success message. |
| Line 94 | **`dashboard.unmatchedTransactions`** – If the API ever omits this field, `.length` will throw. Type says it exists. | Low | Use optional chaining: `dashboard.unmatchedTransactions?.length > 0`. |

---

## 18. pages/reports/ReportsPage.tsx

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Lines 19–22, 26–29 | **Empty `catch` blocks** – `loadPnl` and `loadVat` swallow errors; user gets no feedback when the report fails. | Medium | In `catch`: `message.error(err?.response?.data?.message ?? 'Failed to load report');` (and import `message` from `antd`). |
| Line 44 | **RangePicker `onChange`** – `d && setDates([d[0]!, d[1]!])` uses non-null assertions. If the picker passes a tuple with nulls in rare cases, this could throw. | Low | Use `d?.[0] != null && d?.[1] != null && setDates([d[0], d[1]])` (with correct types) to avoid asserting. |

---

## 19. pages/settings/SettingsPage.tsx

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Line 10 | **Organization state never read** – `const [, setOrg] = useState(...)`; only `setOrg` and form are used. No loading/error state for organization. | Low | Either use `org` for loading/error UI or remove the state and only use `form.setFieldsValue(res.data)` (and optionally a loading flag for the form). |
| Lines 17–21 | **No `.catch()` on any of the four API calls** – If any of `getOrganization`, `getPaymentSources`, `getRates`, or `getLogs` fail, the user sees no error and form/tables may be empty or stale. | Medium | Add `.catch((err) => message.error(err?.response?.data?.message ?? 'Failed to load settings'));` to each, and consider a single loading state for the tab content. |
| Line 18 | **`form.setFieldsValue(res.data)`** – If the API returns null/undefined or a shape that doesn’t match form field names, some fields may not be set or may get wrong values. | Low | Validate `res.data` and optionally use a type guard or map known fields only. |
| Line 25 | **`handleSaveOrg`** – Generic `message.error('Failed')`; server message is ignored. | Low | Use `message.error(err?.response?.data?.message ?? 'Failed');` in the catch block. |
| useEffect deps | **Missing `form` in dependency array** – ESLint may warn. Ant Design’s `form` reference is stable, so this is optional. | Low | Add `form` to the dependency array if you want to satisfy exhaustive-deps. |

---

## 20. Routing (App.tsx)

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| — | Routes and `ProtectedRoute` are correctly structured. Index redirect and nested layout work. | — | — |

---

## 21. Redux state management

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| authSlice | **No `fetchMe.pending`** – No loading flag for “fetch current user”; UI can’t show a loading state during that request. | Low | Add `.addCase(fetchMe.pending, (state) => { state.loading = true; })` and set `loading = false` in fulfilled/rejected if you want a single loading flag for auth. |
| — | Otherwise slice and selectors are consistent. | — | — |

---

## 22. Ant Design usage

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Tables | **`dataSource` and `rowKey="id"`** – Used consistently; fine as long as every item has a unique `id`. | — | — |
| Modals | **`onOk={() => form.submit()}`** – Correct pattern; validation runs on submit. | — | — |
| Form.List | Journals and Invoices use `Form.List` correctly; keys and names are used properly. | — | — |
| ReconciliationPage Line 75–79 | **Multiple `<Upload>` in `extra`** – `bankAccounts.map` returns an array of Upload buttons without a wrapper; React expects a single element or keyed fragments. May cause key warnings or layout issues. | Medium | Wrap in a fragment or Space: `extra={<Space>{bankAccounts.map(b => (...))}</Space>}` and ensure each item has a stable `key`. |

---

## 23. TypeScript and imports

| Area | Issue | Severity | Suggested fix |
|------|--------|----------|----------------|
| Various | **`any` in handlers** – Several form submit handlers and catch blocks use `values: any` or `err: any`. | Low | Replace with proper types (e.g. `RegisterRequest`, `AxiosError`, or form value types). |
| — | No missing or incorrect imports detected in the files reviewed. | — | — |

---

## Summary by severity

- **Medium** – Fix soon: root null check (main), API error handling and user feedback (dashboard, accounts, journals, expenses, invoices, sales, reconciliation, reports, settings), error message extraction (no `[object Object]`), `revenueChart` null safety, Reconciliation loading/import type, Reports empty catch, query param encoding, import response type, Reconciliation Upload extra layout.
- **Low** – Nice to have: authSlice `any`/payload typing, formatPercent negative sign, formatDate invalid input, Register form types/messages, defensive date/lines checks, SalesPage `selectedDate` guard, Settings org state and form deps, Redux fetchMe pending, Ant Design Upload extra wrapper, replacing remaining `any` types.

---

## Recommended order of fixes

1. Add `.catch()` and user-visible error messages to all API calls that currently lack them.
2. Standardize error message extraction (e.g. `err?.response?.data?.message ?? (typeof err?.response?.data === 'string' ? err.response.data : '...')`) everywhere.
3. Type `importStatement` response and optionally guard `res.data.imported`.
4. Add null/empty fallback for `data.revenueChart` in DashboardPage.
5. Use loading state in ReconciliationPage and handle Promise.all rejection.
6. Fix `formatPercent` for negative values.
7. Add null check for `#root` in main.tsx.
8. Encode optional query parameters in api/index.ts.
9. Address remaining low-severity items (types, defensive checks, Ant Design extra wrapper) as you touch those files.
