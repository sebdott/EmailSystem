import React, {Component} from 'react';
import {connect} from 'dva';
import css from '../../styles/component/MainLayout/Header.less';
import logo from '../../assets/logo.png';
class Header extends Component {
  render() {
    return (
      <div>
        <div className={css.headerLogo}>
          <img alt="mainLogo" src={logo} height="300" width="300" />
        </div>
        <div className={css.headerTitle}>
          <h1>Emailing System</h1>
        </div>
      </div>
    );
  }
}

const mapStatesToProps = ({userModel}) => {
  return {};
};

export default connect(mapStatesToProps)(Header);
