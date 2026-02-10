import React, { useEffect, useState } from 'react';
import { Table, Button, Typography, Tag, Modal, Form, Input, InputNumber, Select, DatePicker, message } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { expensesApi, vendorsApi } from '../../api';
import { formatCurrency, formatDate } from '../../utils/format';
import type { ExpenseDto, VendorDto } from '../../types';

const { Title } = Typography;

const categories = ['Material', 'Personnel', 'Rent', 'Utilities', 'Insurance', 'Marketing', 'Administrative', 'Maintenance', 'Licenses', 'ProfessionalServices', 'CreditCardFees', 'Miscellaneous'];
const paymentMethods = ['Cash', 'Card', 'BankTransfer', 'Online', 'Other'];
const vatRates = [{ value: 'Standard', label: '8.1% Standard' }, { value: 'Reduced', label: '2.6% Reduced' }, { value: 'Hospitality', label: '3.8% Hospitality' }, { value: 'Exempt', label: '0% Exempt' }];

const ExpensesPage: React.FC = () => {
  const [expenses, setExpenses] = useState<ExpenseDto[]>([]);
  const [vendors, setVendors] = useState<VendorDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [modalOpen, setModalOpen] = useState(false);
  const [form] = Form.useForm();

  const load = (p = page) => {
    setLoading(true);
    expensesApi.getAll(p, 20).then(res => { setExpenses(res.data.items); setTotal(res.data.totalCount); }).finally(() => setLoading(false));
  };

  useEffect(() => { load(); vendorsApi.getAll().then(res => setVendors(res.data)); }, []);

  const handleCreate = async (values: any) => {
    try {
      await expensesApi.create({ ...values, expenseDate: values.expenseDate.format('YYYY-MM-DD') });
      message.success('Expense created'); setModalOpen(false); form.resetFields(); load();
    } catch (err: any) { message.error(err.response?.data?.error || 'Failed'); }
  };

  const columns = [
    { title: 'Date', dataIndex: 'expenseDate', key: 'date', width: 110, render: (d: string) => formatDate(d) },
    { title: 'Description', dataIndex: 'description', key: 'desc' },
    { title: 'Category', dataIndex: 'category', key: 'cat', render: (c: string) => <Tag>{c}</Tag> },
    { title: 'Vendor', dataIndex: 'vendorName', key: 'vendor' },
    { title: 'Net', dataIndex: 'netAmount', key: 'net', align: 'right' as const, render: (v: number) => formatCurrency(v) },
    { title: 'VAT', dataIndex: 'vatAmount', key: 'vat', align: 'right' as const, render: (v: number) => formatCurrency(v) },
    { title: 'Total', dataIndex: 'amount', key: 'total', align: 'right' as const, render: (v: number) => <strong>{formatCurrency(v)}</strong> },
  ];

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <Title level={3} style={{ margin: 0 }}>Expenses & Bills</Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => setModalOpen(true)}>New Expense</Button>
      </div>
      <Table dataSource={expenses} columns={columns} rowKey="id" loading={loading} size="small" pagination={{ total, current: page, pageSize: 20, onChange: (p) => { setPage(p); load(p); } }} />

      <Modal title="New Expense" open={modalOpen} onCancel={() => setModalOpen(false)} onOk={() => form.submit()} okText="Create" width={560}>
        <Form form={form} layout="vertical" onFinish={handleCreate}>
          <Form.Item name="expenseDate" label="Date" rules={[{ required: true }]}><DatePicker style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="description" label="Description" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="amount" label="Total Amount (incl. VAT)" rules={[{ required: true }]}><InputNumber style={{ width: '100%' }} min={0} precision={2} prefix="CHF" /></Form.Item>
          <Form.Item name="vatRate" label="VAT Rate" rules={[{ required: true }]}><Select options={vatRates} /></Form.Item>
          <Form.Item name="category" label="Category" rules={[{ required: true }]}><Select options={categories.map(c => ({ value: c, label: c }))} /></Form.Item>
          <Form.Item name="paymentMethod" label="Payment Method" rules={[{ required: true }]}><Select options={paymentMethods.map(m => ({ value: m, label: m }))} /></Form.Item>
          <Form.Item name="vendorId" label="Vendor"><Select allowClear placeholder="Select vendor" options={vendors.map(v => ({ value: v.id, label: v.name }))} /></Form.Item>
          <Form.Item name="notes" label="Notes"><Input.TextArea rows={2} /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default ExpensesPage;
