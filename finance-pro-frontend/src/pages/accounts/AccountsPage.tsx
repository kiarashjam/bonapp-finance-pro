import React, { useEffect, useState } from 'react';
import { Table, Button, Typography, Tag, Modal, Form, Input, Select, message } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { accountsApi } from '../../api';
import type { LedgerAccountDto } from '../../types';

const { Title } = Typography;

const AccountsPage: React.FC = () => {
  const [accounts, setAccounts] = useState<LedgerAccountDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [form] = Form.useForm();

  const load = () => {
    setLoading(true);
    accountsApi.getAll().then(res => setAccounts(res.data)).finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, []);

  const handleCreate = async (values: any) => {
    try {
      await accountsApi.create(values);
      message.success('Account created');
      setModalOpen(false);
      form.resetFields();
      load();
    } catch (err: any) {
      message.error(err.response?.data?.error || 'Failed to create account');
    }
  };

  const columns = [
    { title: 'Account #', dataIndex: 'accountNumber', key: 'accountNumber', width: 120, sorter: (a: LedgerAccountDto, b: LedgerAccountDto) => a.accountNumber.localeCompare(b.accountNumber) },
    { title: 'Name', dataIndex: 'name', key: 'name' },
    { title: 'Type', dataIndex: 'accountType', key: 'accountType', render: (t: string) => <Tag>{t}</Tag> },
    { title: 'Status', key: 'status', render: (_: any, r: LedgerAccountDto) => <Tag color={r.isActive ? 'green' : 'default'}>{r.isActive ? 'Active' : 'Inactive'}</Tag> },
    { title: 'System', key: 'system', render: (_: any, r: LedgerAccountDto) => r.isSystemAccount ? <Tag color="blue">System</Tag> : null },
  ];

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <Title level={3} style={{ margin: 0 }}>Chart of Accounts</Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => setModalOpen(true)}>Add Account</Button>
      </div>
      <Table dataSource={accounts} columns={columns} rowKey="id" loading={loading} size="small" pagination={false} />

      <Modal title="New Account" open={modalOpen} onCancel={() => setModalOpen(false)} onOk={() => form.submit()} okText="Create">
        <Form form={form} layout="vertical" onFinish={handleCreate}>
          <Form.Item name="accountNumber" label="Account Number" rules={[{ required: true }]}>
            <Input placeholder="e.g. 1030" />
          </Form.Item>
          <Form.Item name="name" label="Account Name" rules={[{ required: true }]}>
            <Input placeholder="e.g. Petty Cash" />
          </Form.Item>
          <Form.Item name="accountType" label="Type" rules={[{ required: true }]}>
            <Select options={[{ value: 'Asset', label: 'Asset' }, { value: 'Liability', label: 'Liability' }, { value: 'Equity', label: 'Equity' }, { value: 'Revenue', label: 'Revenue' }, { value: 'Expense', label: 'Expense' }]} />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input.TextArea rows={2} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default AccountsPage;
