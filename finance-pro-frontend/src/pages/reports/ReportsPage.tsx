import React, { useState } from 'react';
import { Card, Typography, DatePicker, Button, Table, Row, Col, Statistic, Divider, Space, Tabs, message } from 'antd';
import { reportsApi } from '../../api';
import { formatCurrency, formatPercent } from '../../utils/format';
import type { ProfitAndLossDto, VatReportDto } from '../../types';
import dayjs from 'dayjs';

const { Title, Text } = Typography;
const { RangePicker } = DatePicker;

const ReportsPage: React.FC = () => {
  const [pnl, setPnl] = useState<ProfitAndLossDto | null>(null);
  const [vat, setVat] = useState<VatReportDto | null>(null);
  const [dates, setDates] = useState<[dayjs.Dayjs, dayjs.Dayjs]>([dayjs().startOf('month'), dayjs()]);
  const [loading, setLoading] = useState(false);

  const loadPnl = async () => {
    setLoading(true);
    try {
      const res = await reportsApi.getProfitAndLoss(dates[0].format('YYYY-MM-DD'), dates[1].format('YYYY-MM-DD'));
      setPnl(res.data);
    } catch (err: any) { message.error(err.response?.data?.error || 'Failed to load P&L report'); } finally { setLoading(false); }
  };

  const loadVat = async () => {
    setLoading(true);
    try {
      const res = await reportsApi.getVatReport(dates[0].format('YYYY-MM-DD'), dates[1].format('YYYY-MM-DD'));
      setVat(res.data);
    } catch (err: any) { message.error(err.response?.data?.error || 'Failed to load VAT report'); } finally { setLoading(false); }
  };

  const pnlLineColumns = [
    { title: 'Account', dataIndex: 'accountNumber', key: 'num', width: 100 },
    { title: 'Name', dataIndex: 'accountName', key: 'name' },
    { title: 'Amount', dataIndex: 'amount', key: 'amount', align: 'right' as const, render: (v: number) => formatCurrency(v) },
  ];

  const items = [
    {
      key: 'pnl', label: 'Profit & Loss',
      children: (
        <div>
          <Space style={{ marginBottom: 16 }}>
            <RangePicker value={dates} onChange={(d) => d && setDates([d[0]!, d[1]!])} />
            <Button type="primary" onClick={loadPnl} loading={loading}>Generate</Button>
          </Space>
          {pnl && (
            <div>
              <Row gutter={[16, 16]}>
                <Col span={6}><Card><Statistic title="Revenue" value={pnl.revenue} prefix="CHF" precision={2} /></Card></Col>
                <Col span={6}><Card><Statistic title="Gross Profit" value={pnl.grossProfit} prefix="CHF" precision={2} suffix={<Text type="secondary" style={{ fontSize: 12 }}> {formatPercent(pnl.grossProfitMargin)}</Text>} /></Card></Col>
                <Col span={6}><Card><Statistic title="Prime Cost" value={pnl.primeCost} prefix="CHF" precision={2} valueStyle={{ color: pnl.primeCostPercent > 65 ? '#cf1322' : '#3f8600' }} suffix={<Text style={{ fontSize: 12 }}> {formatPercent(pnl.primeCostPercent)}</Text>} /></Card></Col>
                <Col span={6}><Card><Statistic title="Net Profit" value={pnl.netOperatingProfit} prefix="CHF" precision={2} valueStyle={{ color: pnl.netOperatingProfit >= 0 ? '#3f8600' : '#cf1322' }} /></Card></Col>
              </Row>
              <Divider />
              <Title level={5}>Revenue</Title>
              <Table dataSource={pnl.revenueItems} columns={pnlLineColumns} rowKey="accountNumber" size="small" pagination={false} />
              <Title level={5} style={{ marginTop: 16 }}>COGS (Cost of Goods Sold)</Title>
              <Table dataSource={pnl.cogsItems} columns={pnlLineColumns} rowKey="accountNumber" size="small" pagination={false} />
              <Title level={5} style={{ marginTop: 16 }}>Labor Costs</Title>
              <Table dataSource={pnl.laborItems} columns={pnlLineColumns} rowKey="accountNumber" size="small" pagination={false} />
              <Title level={5} style={{ marginTop: 16 }}>Operating Expenses</Title>
              <Table dataSource={pnl.opExItems} columns={pnlLineColumns} rowKey="accountNumber" size="small" pagination={false} />
            </div>
          )}
        </div>
      ),
    },
    {
      key: 'vat', label: 'VAT Report',
      children: (
        <div>
          <Space style={{ marginBottom: 16 }}>
            <RangePicker value={dates} onChange={(d) => d && setDates([d[0]!, d[1]!])} />
            <Button type="primary" onClick={loadVat} loading={loading}>Generate</Button>
          </Space>
          {vat && (
            <Row gutter={[16, 16]}>
              <Col span={6}><Card><Statistic title="Total Revenue" value={vat.totalRevenue} prefix="CHF" precision={2} /></Card></Col>
              <Col span={6}><Card><Statistic title="Output VAT" value={vat.totalOutputVat} prefix="CHF" precision={2} /></Card></Col>
              <Col span={6}><Card><Statistic title="Input VAT" value={vat.totalInputVat} prefix="CHF" precision={2} /></Card></Col>
              <Col span={6}><Card><Statistic title="Net VAT Payable" value={vat.netVatPayable} prefix="CHF" precision={2} valueStyle={{ color: '#cf1322', fontWeight: 'bold' }} /></Card></Col>
              <Col span={24}>
                <Card title="VAT Breakdown">
                  <Row gutter={16}>
                    <Col span={8}><Statistic title="Standard (8.1%)" value={vat.outputVatStandard} prefix="CHF" precision={2} /></Col>
                    <Col span={8}><Statistic title="Reduced (2.6%)" value={vat.outputVatReduced} prefix="CHF" precision={2} /></Col>
                    <Col span={8}><Statistic title="Hospitality (3.8%)" value={vat.outputVatHospitality} prefix="CHF" precision={2} /></Col>
                  </Row>
                </Card>
              </Col>
            </Row>
          )}
        </div>
      ),
    },
  ];

  return (
    <div>
      <Title level={3}>Reports</Title>
      <Tabs items={items} />
    </div>
  );
};

export default ReportsPage;
