import React, { useEffect, useState } from 'react';
import { Form, Input, Button, Typography, Tabs, Table, Tag, message, Select } from 'antd';
import { SaveOutlined } from '@ant-design/icons';
import { settingsApi, taxApi, auditApi } from '../../api';
import { formatDate } from '../../utils/format';
import type { OrganizationSettingsDto, PaymentSourceDto, TaxRateDto, AuditLogDto } from '../../types';

const { Title } = Typography;

const SettingsPage: React.FC = () => {
  const [, setOrg] = useState<OrganizationSettingsDto | null>(null);
  const [sources, setSources] = useState<PaymentSourceDto[]>([]);
  const [rates, setRates] = useState<TaxRateDto[]>([]);
  const [logs, setLogs] = useState<AuditLogDto[]>([]);
  const [form] = Form.useForm();

  useEffect(() => {
    settingsApi.getOrganization().then(res => { setOrg(res.data); form.setFieldsValue(res.data); }).catch(() => message.error('Failed to load organization settings'));
    settingsApi.getPaymentSources().then(res => setSources(res.data)).catch(() => {});
    taxApi.getRates().then(res => setRates(res.data)).catch(() => {});
    auditApi.getLogs(1, 50).then(res => setLogs(res.data.items)).catch(() => {});
  }, []);

  const handleSaveOrg = async (values: any) => {
    try { await settingsApi.updateOrganization(values); message.success('Settings saved'); } catch (err: any) { message.error(err.response?.data?.error || 'Failed to save settings'); }
  };

  const items = [
    {
      key: 'org', label: 'Organization',
      children: (
        <Form form={form} layout="vertical" onFinish={handleSaveOrg} style={{ maxWidth: 600 }}>
          <Form.Item name="name" label="Business Name" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="legalName" label="Legal Name"><Input /></Form.Item>
          <div style={{ display: 'flex', gap: 12 }}>
            <Form.Item name="street" label="Street" style={{ flex: 2 }}><Input /></Form.Item>
            <Form.Item name="houseNumber" label="No." style={{ flex: 1 }}><Input /></Form.Item>
          </div>
          <div style={{ display: 'flex', gap: 12 }}>
            <Form.Item name="postalCode" label="Postal Code" style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="city" label="City" style={{ flex: 2 }}><Input /></Form.Item>
          </div>
          <div style={{ display: 'flex', gap: 12 }}>
            <Form.Item name="phone" label="Phone" style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="email" label="Email" style={{ flex: 1 }}><Input /></Form.Item>
          </div>
          <Form.Item name="vatId" label="VAT ID"><Input placeholder="CHE-xxx.xxx.xxx" /></Form.Item>
          <Form.Item name="iban" label="IBAN"><Input placeholder="CH00 0000 0000 0000 0000 0" /></Form.Item>
          <Form.Item name="qrIban" label="QR-IBAN"><Input placeholder="CH00 3000 0000 0000 0000 0" /></Form.Item>
          <div style={{ display: 'flex', gap: 12 }}>
            <Form.Item name="currency" label="Currency" style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="defaultLanguage" label="Language" style={{ flex: 1 }}>
              <Select options={[{ value: 'de', label: 'Deutsch' }, { value: 'fr', label: 'Français' }, { value: 'it', label: 'Italiano' }, { value: 'en', label: 'English' }]} />
            </Form.Item>
            <Form.Item name="fiscalYearStartMonth" label="Fiscal Year Start" style={{ flex: 1 }}>
              <Select options={Array.from({ length: 12 }, (_, i) => ({ value: i + 1, label: new Date(2024, i).toLocaleString('en', { month: 'long' }) }))} />
            </Form.Item>
          </div>
          <Button type="primary" htmlType="submit" icon={<SaveOutlined />}>Save Settings</Button>
        </Form>
      ),
    },
    {
      key: 'sources', label: 'Payment Sources',
      children: (
        <Table dataSource={sources} rowKey="id" size="small" columns={[
          { title: 'Name', dataIndex: 'name' },
          { title: 'Type', dataIndex: 'sourceType', render: (t: string) => <Tag>{t}</Tag> },
          { title: 'Active', key: 'active', render: (_: any, r: PaymentSourceDto) => <Tag color={r.isActive ? 'green' : 'default'}>{r.isActive ? 'Active' : 'Inactive'}</Tag> },
          { title: 'Created', dataIndex: 'createdAt', render: (d: string) => formatDate(d) },
        ]} />
      ),
    },
    {
      key: 'rates', label: 'Tax Rates',
      children: (
        <Table dataSource={rates} rowKey="id" size="small" columns={[
          { title: 'Name', dataIndex: 'name' },
          { title: 'Type', dataIndex: 'rateType' },
          { title: 'Rate', dataIndex: 'rate', render: (r: number) => `${r}%` },
          { title: 'Since', dataIndex: 'effectiveFrom', render: (d: string) => formatDate(d) },
        ]} />
      ),
    },
    {
      key: 'audit', label: 'Audit Log',
      children: (
        <Table dataSource={logs} rowKey="id" size="small" pagination={{ pageSize: 20 }} columns={[
          { title: 'Timestamp', dataIndex: 'timestamp', render: (d: string) => new Date(d).toLocaleString('de-CH') },
          { title: 'Action', dataIndex: 'action', render: (a: string) => <Tag>{a}</Tag> },
          { title: 'Entity', dataIndex: 'entityType' },
          { title: 'ID', dataIndex: 'entityId' },
          { title: 'User', dataIndex: 'userName' },
        ]} />
      ),
    },
  ];

  return (
    <div>
      <Title level={3}>Settings</Title>
      <Tabs items={items} />
    </div>
  );
};

export default SettingsPage;
