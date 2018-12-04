import React, {Component} from 'react';
import {Table, Button} from 'antd';
import {connect} from 'dva';
import {getLocalDatetimeFromUTC} from '../../utils/';
import Main from '../MainLayout/Main';
import css from '../../styles/component/Email.less';

class EmailIndex extends Component {
  constructor(props) {
    super(props);
    this.dispatch = this.props.dispatch;
  }
  componentWillMount() {
    this.dispatch({
      type: 'emailModel/getEmailLogList',
    });
  }
  componentWillUnmount() {
    this.dispatch({
      type: 'emailModel/initializeState',
    });
  }
  onPageChange(page, pageSize) {
    this.dispatch({
      type: 'emailModel/updateState',
      payload: {
        pageSize,
        currentPage: page,
      },
    });
    this.dispatch({
      type: 'emailModel/getEmailLogList',
    });
  }
  onPageSizeChange(current, pageSize) {
    this.onPageChange(current, pageSize);
  }
  renderDateTime(date) {
    if (date) {
      return <div>{getLocalDatetimeFromUTC(date)}</div>;
    }
    return <div />;
  }
  renderTable() {
    const {displayList, pageSize, currentPage} = this.props;
    const columns = [
      {
        title: 'Username',
        dataIndex: 'username',
        key: 'username',
      },
      {
        title: 'Email',
        dataIndex: 'email',
        key: 'email',
      },
      {
        title: 'Sent Email DateTime',
        dataIndex: 'sendEmailDatetime',
        key: 'sendEmailDatetime',
        render: data => this.renderDateTime(data),
      },
    ];

    let dataSource = [];

    _.map(displayList.listofDisplay, (item, key) => {
      const {
        userId,
        username,
        email,
        sendEmailDatetime,
        isSentEmailValidation,
      } = item;

      dataSource.push({
        key: key + '_emailListRow',
        userId,
        username,
        email,
        sendEmailDatetime,
        isSentEmailValidation,
      });
    });

    const pagination = {
      total: displayList.recordTotalCount,
      pageSize,
      current: currentPage,
      showTotal: (total, range) => `${range[0]}-${range[1]} of ${total} items`,
      onChange: this.onPageChange.bind(this),
      onShowSizeChange: this.onPageSizeChange.bind(this),
      showSizeChanger: true,
      pageSizeOptions: ['5', '20', '50', '100'],
    };
    return (
      <div>
        <Table
          pagination={pagination}
          locale={{emptyText: 'No Words'}}
          dataSource={dataSource}
          columns={columns}
        />
      </div>
    );
  }
  onRefreshClick() {
    this.dispatch({
      type: 'emailModel/getEmailLogList',
    });
  }
  renderScene() {
    const {awaitingResponse} = this.props;
    return (
      <div>
        <div className={css.email_buttonOuter}>
          <Button
            className={css.email_button}
            type="primary"
            loading={awaitingResponse}
            onClick={this.onRefreshClick.bind(this)}>
            Refresh Email Log List
          </Button>
        </div>
        {this.renderTable()}
      </div>
    );
  }
  render() {
    const {awaitingResponse} = this.props;
    return (
      <Main content={this.renderScene()} awaitingResponse={awaitingResponse} />
    );
  }
}

const mapStatesToProps = ({emailModel}) => {
  return {
    ...emailModel,
  };
};

export default connect(mapStatesToProps)(EmailIndex);
