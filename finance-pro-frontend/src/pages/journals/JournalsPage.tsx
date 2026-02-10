import React, { useEffect, useState } from 'react';
import { Table, Button, Typography, Tag, Space, Modal, Form, Input, DatePicker, InputNumber, Select, message, Divider } from 'antd';
import { PlusOutlined, MinusCircleOutlined } from '@ant-design/icons';
import { journalsApi, accountsApi } from '../../api';
import { formatCurrency, formatDate, getStatusColor } from '../../utils/format';
import type { JournalEntryDto, LedgerAccountDto } from '../../types';

const { Title } = Typography;

const JournalsPage: React.FC = () => {
  const [entries, setEntries] = useState<JournalEntryDto[]>([]);
  const [accounts, setAccounts] = useState<LedgerAccountDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [modalOpen, setModalOpen] = useState(false);
  const [form] = Form.useForm();

  const load = (p = page) => {
    setLoading(true);
    journalsApi.getAll(p, 20).then(res => { setEntries(res.data.items); setTotal(res.data.totalCount); }).finally(() => setLoading(false));
  };

  useEffect(() => { load(); accountsApi.getAll().then(res => setAccounts(res.data)); }, []);

  const handleCreate = async (values: any) => {
    try {
      const data = {
        entryDate: values.entryDate.format('YYYY-MM-DD'),
        description: values.description,
        lines: values.lines.map((l: any) => ({ ledgerAccountId: l.accountId, debitAmount: l.debit || 0, creditAmount: l.credit || 0, description: l.lineDescription })),
      };
      await journalsApi.create(data);
      message.success('Journal entry created');
      setModalOpen(false);
      form.resetFields();
      load();
    } catch (err: any) {
      message.error(err.response?.data?.error || 'Failed to create entry');
    }
  };

  const handlePost = async (id: number) => {
    try { await journalsApi.post(id); message.success('Entry posted'); load(); } catch (err: any) { message.error(err.response?.data?.error || 'Failed'); }
  };

  const columns = [
    { title: 'Ref #', dataIndex: 'referenceNumber', key: 'ref', width: 150 },
    { title: 'Date', dataIndex: 'entryDate', key: 'date', width: 110, render: (d: string) => formatDate(d) },
    { title: 'Description', dataIndex: 'description', key: 'desc' },
    { title: 'Debit', dataIndex: 'totalDebit', key: 'debit', align: 'right' as const, render: (v: number) => formatCurrency(v) },
    { title: 'Credit', dataIndex: 'totalCredit', key: 'credit', align: 'right' as const, render: (v: number) => formatCurrency(v) },
    { title: 'Status', dataIndex: 'status', key: 'status', render: (s: string) => <Tag color={getStatusColor(s)}>{s}</Tag> },
    { title: '', key: 'actions', width: 100, render: (_: any, r: JournalEntryDto) => r.status === 'Draft' ? <Button size="small" type="link" onClick={() => handlePost(r.id)}>Post</Button> : null },
  ];

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <Title level={3} style={{ margin: 0 }}>Journal Entries</Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => setModalOpen(true)}>New Entry</Button>
      </div>
      <Table dataSource={entries} columns={columns} rowKey="id" loading={loading} size="small" pagination={{ total, current: page, pageSize: 20, onChange: (p) => { setPage(p); load(p); } }} />

      <Modal title="New Journal Entry" open={modalOpen} onCancel={() => setModalOpen(false)} onOk={() => form.submit()} width={720} okText="Create">
        <Form form={form} layout="vertical" onFinish={handleCreate} initialValues={{ lines: [{ debit: 0, credit: 0 }, { debit: 0, credit: 0 }] }}>
          <Form.Item name="entryDate" label="Date" rules={[{ required: true }]}>
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input placeholder="Entry description" />
          </Form.Item>
          <Divider>Lines (Debit must equal Credit)</Divider>
          <Form.List name="lines">
            {(fields, { add, remove }) => (
              <>
                {fields.map(({ key, name, ...rest }) => (
                  <Space key={key} style={{ display: 'flex', marginBottom: 8 }} align="baseline">
                    <Form.Item {...rest} name={[name, 'accountId']} rules={[{ required: true }]}>
                      <Select placeholder="Account" style={{ width: 220 }} showSearch optionFilterProp="label" options={accounts.map(a => ({ value: a.id, label: `${a.accountNumber} - ${a.name}` }))} />
                    </Form.Item>
                    <Form.Item {...rest} name={[name, 'debit']}><InputNumber placeholder="Debit" min={0} precision={2} style={{ width: 120 }} /></Form.Item>
                    <Form.Item {...rest} name={[name, 'credit']}><InputNumber placeholder="Credit" min={0} precision={2} style={{ width: 120 }} /></Form.Item>
                    <MinusCircleOutlined onClick={() => remove(name)} />
                  </Space>
                ))}
                <Button type="dashed" onClick={() => add({ debit: 0, credit: 0 })} block icon={<PlusOutlined />}>Add Line</Button>
              </>
            )}
          </Form.List>
        </Form>
      </Modal>
    </div>
  );
};

export default JournalsPage;
