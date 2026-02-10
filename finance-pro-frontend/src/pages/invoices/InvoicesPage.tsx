import React, { useEffect, useState } from 'react';
import { Table, Button, Typography, Tag, Space, Modal, Form, Input, InputNumber, Select, DatePicker, Divider, message } from 'antd';
import { PlusOutlined, MinusCircleOutlined } from '@ant-design/icons';
import { invoicesApi, customersApi } from '../../api';
import { formatCurrency, formatDate, getStatusColor } from '../../utils/format';
import type { InvoiceDto, CustomerDto } from '../../types';

const { Title } = Typography;
const vatRates = [{ value: 'Standard', label: '8.1%' }, { value: 'Reduced', label: '2.6%' }, { value: 'Hospitality', label: '3.8%' }, { value: 'Exempt', label: '0%' }];

const InvoicesPage: React.FC = () => {
  const [invoices, setInvoices] = useState<InvoiceDto[]>([]);
  const [customers, setCustomers] = useState<CustomerDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [modalOpen, setModalOpen] = useState(false);
  const [form] = Form.useForm();

  const load = (p = page) => {
    setLoading(true);
    invoicesApi.getAll(p, 20).then(res => { setInvoices(res.data.items); setTotal(res.data.totalCount); }).finally(() => setLoading(false));
  };

  useEffect(() => { load(); customersApi.getAll().then(res => setCustomers(res.data)); }, []);

  const handleCreate = async (values: any) => {
    try {
      await invoicesApi.create({
        customerId: values.customerId,
        invoiceDate: values.invoiceDate.format('YYYY-MM-DD'),
        dueDate: values.dueDate.format('YYYY-MM-DD'),
        notes: values.notes,
        lines: values.lines.map((l: any, i: number) => ({ ...l, sortOrder: i + 1 })),
      });
      message.success('Invoice created'); setModalOpen(false); form.resetFields(); load();
    } catch (err: any) { message.error(err.response?.data?.error || 'Failed'); }
  };

  const handleStatusChange = async (id: number, status: string) => {
    try { await invoicesApi.updateStatus(id, status); message.success('Status updated'); load(); } catch (err: any) { message.error(err.response?.data?.error || 'Failed'); }
  };

  const columns = [
    { title: 'Invoice #', dataIndex: 'invoiceNumber', key: 'num', width: 140 },
    { title: 'Customer', dataIndex: 'customerName', key: 'customer' },
    { title: 'Date', dataIndex: 'invoiceDate', key: 'date', width: 110, render: (d: string) => formatDate(d) },
    { title: 'Due', dataIndex: 'dueDate', key: 'due', width: 110, render: (d: string) => formatDate(d) },
    { title: 'Total', dataIndex: 'total', key: 'total', align: 'right' as const, render: (v: number) => <strong>{formatCurrency(v)}</strong> },
    { title: 'Paid', dataIndex: 'paidAmount', key: 'paid', align: 'right' as const, render: (v: number) => formatCurrency(v) },
    { title: 'Outstanding', dataIndex: 'outstandingAmount', key: 'out', align: 'right' as const, render: (v: number) => v > 0 ? <span style={{ color: '#cf1322' }}>{formatCurrency(v)}</span> : formatCurrency(v) },
    { title: 'Status', dataIndex: 'status', key: 'status', render: (s: string) => <Tag color={getStatusColor(s)}>{s}</Tag> },
    { title: '', key: 'actions', width: 120, render: (_: any, r: InvoiceDto) => (
      <Space>
        {r.status === 'Draft' && <Button size="small" type="link" onClick={() => handleStatusChange(r.id, 'Sent')}>Send</Button>}
        {r.status === 'Sent' && <Button size="small" type="link" onClick={() => handleStatusChange(r.id, 'Paid')}>Mark Paid</Button>}
      </Space>
    )},
  ];

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <Title level={3} style={{ margin: 0 }}>Invoices</Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => setModalOpen(true)}>New Invoice</Button>
      </div>
      <Table dataSource={invoices} columns={columns} rowKey="id" loading={loading} size="small" pagination={{ total, current: page, pageSize: 20, onChange: (p) => { setPage(p); load(p); } }} />

      <Modal title="New Invoice" open={modalOpen} onCancel={() => setModalOpen(false)} onOk={() => form.submit()} width={780} okText="Create">
        <Form form={form} layout="vertical" onFinish={handleCreate} initialValues={{ lines: [{ quantity: 1 }] }}>
          <Form.Item name="customerId" label="Customer" rules={[{ required: true }]}>
            <Select showSearch optionFilterProp="label" options={customers.map(c => ({ value: c.id, label: c.name }))} />
          </Form.Item>
          <Space>
            <Form.Item name="invoiceDate" label="Invoice Date" rules={[{ required: true }]}><DatePicker /></Form.Item>
            <Form.Item name="dueDate" label="Due Date" rules={[{ required: true }]}><DatePicker /></Form.Item>
          </Space>
          <Divider>Line Items</Divider>
          <Form.List name="lines">
            {(fields, { add, remove }) => (
              <>
                {fields.map(({ key, name, ...rest }) => (
                  <Space key={key} style={{ display: 'flex', marginBottom: 8 }} align="baseline">
                    <Form.Item {...rest} name={[name, 'description']} rules={[{ required: true }]}><Input placeholder="Description" style={{ width: 200 }} /></Form.Item>
                    <Form.Item {...rest} name={[name, 'quantity']}><InputNumber placeholder="Qty" min={0.01} style={{ width: 80 }} /></Form.Item>
                    <Form.Item {...rest} name={[name, 'unitPrice']} rules={[{ required: true }]}><InputNumber placeholder="Price" min={0} precision={2} style={{ width: 110 }} /></Form.Item>
                    <Form.Item {...rest} name={[name, 'vatRate']} rules={[{ required: true }]}><Select options={vatRates} style={{ width: 100 }} /></Form.Item>
                    <MinusCircleOutlined onClick={() => remove(name)} />
                  </Space>
                ))}
                <Button type="dashed" onClick={() => add({ quantity: 1 })} block icon={<PlusOutlined />}>Add Line</Button>
              </>
            )}
          </Form.List>
          <Form.Item name="notes" label="Notes" style={{ marginTop: 16 }}><Input.TextArea rows={2} /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default InvoicesPage;
