import React, { useEffect, useState } from 'react';
import { Card, Row, Col, Statistic, Table, Typography, Tag, Button, Upload, message, Progress, Spin } from 'antd';
import { BankOutlined, UploadOutlined, CheckCircleOutlined, WarningOutlined } from '@ant-design/icons';
import { reconciliationApi } from '../../api';
import { formatCurrency, formatDate } from '../../utils/format';
import type { ReconciliationDashboardDto, BankAccountDto } from '../../types';

const { Title } = Typography;

const ReconciliationPage: React.FC = () => {
  const [dashboard, setDashboard] = useState<ReconciliationDashboardDto | null>(null);
  const [bankAccounts, setBankAccounts] = useState<BankAccountDto[]>([]);
  const [loading, setLoading] = useState(true);

  const load = () => {
    setLoading(true);
    Promise.all([reconciliationApi.getDashboard(), reconciliationApi.getBankAccounts()])
      .then(([dash, banks]) => { setDashboard(dash.data); setBankAccounts(banks.data); })
      .catch(() => message.error('Failed to load reconciliation data'))
      .finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, []);

  const handleImport = async (bankAccountId: number, file: File) => {
    try {
      const res = await reconciliationApi.importStatement(bankAccountId, file);
      message.success(`Imported ${res.data.imported} transactions`);
      load();
    } catch (err: any) { message.error('Import failed'); }
  };

  const payoutColumns = [
    { title: 'Provider', dataIndex: 'paymentSourceName', key: 'provider' },
    { title: 'Expected', dataIndex: 'expectedAmount', key: 'expected', align: 'right' as const, render: (v: number) => formatCurrency(v) },
    { title: 'Actual', dataIndex: 'actualAmount', key: 'actual', align: 'right' as const, render: (v: number | null) => v ? formatCurrency(v) : '—' },
    { title: 'Expected Date', dataIndex: 'expectedDate', key: 'date', render: (d: string) => formatDate(d) },
    { title: 'Status', dataIndex: 'status', key: 'status', render: (s: string) => {
      const color = s === 'Received' ? 'green' : s === 'Pending' ? 'orange' : s === 'Missing' ? 'red' : 'red';
      return <Tag color={color}>{s}</Tag>;
    }},
  ];

  const unmatchedColumns = [
    { title: 'Date', dataIndex: 'transactionDate', key: 'date', render: (d: string) => formatDate(d) },
    { title: 'Description', dataIndex: 'description', key: 'desc' },
    { title: 'Amount', dataIndex: 'amount', key: 'amount', align: 'right' as const, render: (v: number) => <strong>{formatCurrency(v)}</strong> },
    { title: 'Reference', dataIndex: 'reference', key: 'ref' },
  ];

  if (loading) return <Spin size="large" style={{ display: 'block', margin: '100px auto' }} />;

  return (
    <div>
      <Title level={3}>Bank Reconciliation</Title>

      {dashboard && (
        <>
          <Row gutter={[16, 16]}>
            <Col xs={24} sm={12} lg={6}>
              <Card><Statistic title="Expected Payouts" value={dashboard.totalExpectedPayouts} prefix="CHF" precision={2} /></Card>
            </Col>
            <Col xs={24} sm={12} lg={6}>
              <Card><Statistic title="Received" value={dashboard.totalReceivedPayouts} precision={2} valueStyle={{ color: '#3f8600' }} prefix={<CheckCircleOutlined />} /></Card>
            </Col>
            <Col xs={24} sm={12} lg={6}>
              <Card><Statistic title="Missing / Pending" value={dashboard.totalMissingPayouts} precision={2} valueStyle={{ color: '#cf1322' }} prefix={<WarningOutlined />} /></Card>
            </Col>
            <Col xs={24} sm={12} lg={6}>
              <Card>
                <Statistic title="Reconciliation Rate" value={dashboard.reconciliationRate} suffix="%" precision={1} />
                <Progress percent={dashboard.reconciliationRate} showInfo={false} strokeColor={dashboard.reconciliationRate > 90 ? '#52c41a' : '#faad14'} />
              </Card>
            </Col>
          </Row>

          <Row gutter={16} style={{ marginTop: 16 }}>
            <Col xs={24}>
              <Card title="Bank Accounts" extra={bankAccounts.map(b => (
                <Upload key={b.id} accept=".csv" showUploadList={false} customRequest={({ file }) => handleImport(b.id, file as File)}>
                  <Button size="small" icon={<UploadOutlined />}>{b.name}: Import CSV</Button>
                </Upload>
              ))}>
                {bankAccounts.map(b => (
                  <Tag key={b.id} icon={<BankOutlined />} style={{ margin: 4 }}>
                    {b.name} ({b.iban}) — Balance: {formatCurrency(b.currentBalance)}
                  </Tag>
                ))}
              </Card>
            </Col>
          </Row>

          <Card title="Recent Payouts" style={{ marginTop: 16 }}>
            <Table dataSource={dashboard.recentPayouts} columns={payoutColumns} rowKey="id" size="small" pagination={false} />
          </Card>

          {dashboard.unmatchedTransactions.length > 0 && (
            <Card title={`Unmatched Bank Transactions (${dashboard.unmatchedBankTransactions})`} style={{ marginTop: 16 }}>
              <Table dataSource={dashboard.unmatchedTransactions} columns={unmatchedColumns} rowKey="id" size="small" pagination={false} />
            </Card>
          )}
        </>
      )}
    </div>
  );
};

export default ReconciliationPage;
