import React, {Component} from 'react';
import {Table, Button} from 'antd';
import {connect} from 'dva';
import {getLocalDatetimeFromUTC, type as Type} from '../../utils/';
import Main from '../MainLayout/Main';
import css from '../../styles/component/User.less';

class UserIndex extends Component {
  constructor(props) {
    super(props);
    this.dispatch = this.props.dispatch;
  }
  componentWillMount() {
    this.dispatch({
      type: 'userModel/getUserList',
    });
  }
  componentWillUnmount() {
    this.dispatch({
      type: 'userModel/initializeState',
    });
  }
  renderStatus(data) {
    if (data !== undefined) {
      if (Type.userStatus[data]) {
        return <div>{Type.userStatus[data]}</div>;
      }
    }
    return <div />;
  }
  renderDateTime(date) {
    if (date) {
      return <div>{getLocalDatetimeFromUTC(date)}</div>;
    }
    return <div />;
  }
  onPageChange(page, pageSize) {
    this.dispatch({
      type: 'userModel/updateState',
      payload: {
        pageSize,
        currentPage: page,
      },
    });
    this.dispatch({
      type: 'userModel/getUserList',
    });
  }
  onPageSizeChange(page, pageSize) {
    this.onPageChange(page, pageSize);
  }
  renderTable() {
    const {displayList, pageSize, currentPage} = this.props;
    const columns = [
      {
        title: 'User ID',
        dataIndex: 'userId',
        key: 'userId',
      },
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
        title: 'Status',
        dataIndex: 'status',
        key: 'status',
        render: data => this.renderStatus(data),
      },
      {
        title: 'Last Modified Date',
        dataIndex: 'lastModifiedDate',
        key: 'lastModifiedDate',
        render: data => this.renderDateTime(data),
      },
      {
        title: 'Last Login Datetime',
        dataIndex: 'lastLoginDatetime',
        key: 'lastLoginDatetime',
        render: data => this.renderDateTime(data),
      },
    ];

    let dataSource = [];

    _.map(displayList.listofDisplay, (item, key) => {
      const {
        userId,
        username,
        email,
        processingID,
        status,
        lastLoginDatetime,
        lastModifiedDate,
      } = item;

      dataSource.push({
        key: key + '_userListRow',
        userId,
        username,
        email,
        processingID,
        status,
        lastLoginDatetime,
        lastModifiedDate,
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
          locale={{emptyText: 'No Users'}}
          dataSource={dataSource}
          columns={columns}
          pagination={pagination}
        />
      </div>
    );
  }
  onRefreshClick() {
    this.dispatch({
      type: 'userModel/getUserList',
    });
  }
  renderScene() {
    const {awaitingResponse} = this.props;
    return (
      <div>
        <div className={css.user_buttonOuter}>
          <Button
            className={css.user_button}
            type="primary"
            loading={awaitingResponse}
            onClick={this.onRefreshClick.bind(this)}>
            Refresh List
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

const mapStatesToProps = ({userModel}) => {
  return {
    ...userModel,
  };
};

export default connect(mapStatesToProps)(UserIndex);
