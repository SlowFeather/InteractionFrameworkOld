namespace XNode.Examples.MathNodes {
    
    public class DisplayValue : XNode.Node {

        /// <summary> 
        /// Create an input port that only allows a single connection.创建只允许单个连接的输入端口。
        /// The backing value is not important, as we are only interested in the input value.支持值并不重要，因为我们只对输入值感兴趣。
        /// We are also acceptable of all input types, so any type will do, as long as it is serializable. 我们也可以接受所有的输入类型，所以任何类型都可以，只要它是可序列化的。
        /// </summary>
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public Anything input;

        /// <summary> Get the value currently plugged in to this node 获取当前插入到此节点的值</summary>
        public object GetValue() {
            return GetInputValue<object>("input");
        }

        /// <summary> This class is defined for the sole purpose of being serializable 定义此类的唯一目的是使其可序列化</summary>
        [System.Serializable] public class Anything {}
    }
}
