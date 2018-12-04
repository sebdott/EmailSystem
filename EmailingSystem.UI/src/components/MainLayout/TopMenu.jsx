import React, {Component} from 'react';
import {Menu, Icon} from 'antd';
import {Link} from 'dva/router';
import {connect} from 'dva';
import {createBrowserHistory as createHistory} from 'history';

class TopMenu extends Component {
  constructor(props) {
    super(props);
    this.dispatch = this.props.dispatch;
  }
  onMenuClick = selectedPage => {
    this.dispatch({
      type: 'navigationModel/updateState',
      payload: {
        currentPage: selectedPage.key,
      },
    });
  };
  renderMenu() {
    const {currentPage} = this.props;
    const {Item} = Menu;
    return _.map(MenuPath, ({Path, Label, IconName}) => {
      return (
        <Item key={Path}>
          <Link to={Path} replace={Path === currentPage}>
            <Icon type={IconName} />
            {Label}
          </Link>
        </Item>
      );
    });
  }
  render() {
    const {location} = createHistory();
    return (
      <Menu
        onClick={this.onMenuClick}
        selectedKeys={[location.hash.substr(1)]}
        mode="horizontal"
        theme="dark">
        {this.renderMenu()}
      </Menu>
    );
  }
}

const MenuPath = {
  Email: {
    Path: '/Email',
    IconName: 'mail',
    Label: 'Email Log',
  },
  User: {
    Path: '/User',
    IconName: 'user',
    Label: 'User List',
  },
  TestControlCenter: {
    Path: '/TestControlCenter',
    IconName: 'laptop',
    Label: 'Test Control Center',
  },
};

const mapStatesToProps = ({navigationModel}) => {
  return {...navigationModel};
};

export default connect(mapStatesToProps)(TopMenu);
