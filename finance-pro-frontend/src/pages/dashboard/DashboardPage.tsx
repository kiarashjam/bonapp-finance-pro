import React, { useEffect, useState } from 'react';
import { Row, Col, Card, Statistic, Typography, Spin } from 'antd';
import { ArrowUpOutlined, ArrowDownOutlined, DollarOutlined, BankOutlined, FileTextOutlined } from '@ant-design/icons';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { reportsApi } from '../../api';
import { formatCurrency } from '../../utils/format';
import type { DashboardDto } from '../../types';

const { Title } = Typography;

const DashboardPage: React.FC = () => {
  const [data, setData] = useState<DashboardDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    reportsApi.getDashboard()
      .then(res => setData(res.data))
      .catch(() => { /* Dashboard data not available yet, show placeholder */ })
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <Spin size="large" style={{ display: 'block', margin: '100px auto' }} />;
  if (!data) return <div>No data available. Start by entering sales or connecting your POS.</div>;

  return (
    <div>
      <Title level={3}>Dashboard</Title>
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={12} lg={6}>
          <Card><Statistic title="Today's Revenue" value={data.todayRevenue} prefix={<DollarOutlined />} precision={2} formatter={(v) => formatCurrency(Number(v))} /></Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card><Statistic title="This Week" value={data.weekRevenue} precision={2} formatter={(v) => formatCurrency(Number(v))} /></Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card><Statistic title="This Month" value={data.monthRevenue} precision={2} formatter={(v) => formatCurrency(Number(v))} /></Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card><Statistic title="Prime Cost" value={data.primeCostPercent} suffix="%" precision={1} valueStyle={{ color: data.primeCostPercent > 65 ? '#cf1322' : '#3f8600' }} prefix={data.primeCostPercent > 65 ? <ArrowUpOutlined /> : <ArrowDownOutlined />} /></Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginTop: 16 }}>
        <Col xs={24} sm={12} lg={6}>
          <Card><Statistic title="Food Cost %" value={data.foodCostPercent} suffix="%" precision={1} /></Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card><Statistic title="Labor Cost %" value={data.laborCostPercent} suffix="%" precision={1} /></Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card><Statistic title="Cash Position" value={data.cashPosition} precision={2} prefix={<BankOutlined />} formatter={(v) => formatCurrency(Number(v))} /></Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card><Statistic title="Outstanding AR" value={data.outstandingAR} precision={2} prefix={<FileTextOutlined />} formatter={(v) => formatCurrency(Number(v))} /></Card>
        </Col>
      </Row>

      <Card style={{ marginTop: 16 }} title="Revenue Trend (Last 60 Days)">
        <ResponsiveContainer width="100%" height={300}>
          <LineChart data={data.revenueChart}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="date" tickFormatter={(d) => new Date(d).toLocaleDateString('de-CH', { day: '2-digit', month: '2-digit' })} />
            <YAxis tickFormatter={(v) => `${(v / 1000).toFixed(0)}k`} />
            <Tooltip formatter={(v) => formatCurrency(Number(v))} labelFormatter={(d) => new Date(d).toLocaleDateString('de-CH')} />
            <Line type="monotone" dataKey="amount" stroke="#1677ff" strokeWidth={2} dot={false} />
          </LineChart>
        </ResponsiveContainer>
      </Card>
    </div>
  );
};

export default DashboardPage;
