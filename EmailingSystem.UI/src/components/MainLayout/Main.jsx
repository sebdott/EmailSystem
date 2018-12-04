import React, {Component} from 'react';
import {connect} from 'dva';
import css from '../../styles/general/layout.less';
import Header from './Header';
import Footer from './Footer';
import TopMenu from './TopMenu';
import {LoadingBar} from '../General';
import {LocaleProvider} from 'antd';
import enUS from 'antd/lib/locale-provider/en_US';

class Main extends Component {
  render() {
    const {content, awaitingResponse} = this.props;
    return (
      <LocaleProvider locale={enUS}>
        <div className={css.pageContainer}>
          <Header />
          <TopMenu />
          <LoadingBar isLoading={awaitingResponse} />
          {content}
          <Footer />
        </div>
      </LocaleProvider>
    );
  }
}

const mapStatesToProps = ({userModel}) => {
  return {};
};

export default connect(mapStatesToProps)(Main);
